/*
    Copyright (C) 2014 Parrot SA
	Copyright (C) 2015 Jeffrey Quesnelle

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in
      the documentation and/or other materials provided with the 
      distribution.
    * Neither the name of Parrot nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
    FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
    COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
    BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
    OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
    AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
*/
#include <errno.h>
#include <stdlib.h>
#include <fcntl.h>

#include <libARSAL/ARSAL_Print.h>
#include <libARSAL/ARSAL_Socket.h>
#include <libARSAL/ARSAL_Sem.h>

#include <libARDiscovery/ARDISCOVERY_Connection.h>
#include "ARDISCOVERY_Connection.h"

#define ARDISCOVERY_CONNECTION_TAG "ARDISCOVERY_Connection"

#define ARDISCOVERY_CONNECTION_TIMEOUT_SEC      5

#define ARDISCOVERY_RECONNECTION_TIME_SEC       1

#define ARDISCOVERY_RECONNECTION_NB_RETRY_MAX   10

/*************************
 * Private header
 *************************/

/**
 * @brief Initializes a socket
 * @param[in] socket socket to intialize
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_CreateSocket (int *socket);

/**
 * @brief Initializes the TCP socket for sending
 * On controller, port is self known and connecting is done first.
 * On device, port is known once received from controller.
 * @param[in] deviceSocket socket used by the device
 * @param[in] port port to receive
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceInitSocket (int *deviceSocket, int port);

/**
 * @brief Connects the Controller TCP socket
 * On controller, port is self known and connecting is done first.
 * On device, port is known once received from controller.
 * @param[in] connectionData connection data
 * @param[in] port port to receive
 * @param[in] ip IP to connect to
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_ControllerInitSocket (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int port, const char *ip);

/**
 * @brief Init a socket
 * @param sockfd The socket descriptor used to connect
 * @param addr The address to connect.
 * @param addrlen The size of the addr
 * @retval On success, ARDISCOVERY_OK is returned. Otherwise, the error is returned
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Socket_Connect(int sockfd, const struct sockaddr *addr, socklen_t addrlen);

/**
 * @brief accepts the connection
 * @param[in] connectionData connection data
 * @param[in] deviceSocket socket used by the device
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceAccept (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int deviceSocket);

/**
 * @brief receives the connection data
 * @param[in] connectionData connection data
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_RxPending (ARDISCOVERY_Connection_ConnectionData_t *connectionData);

/**
 * @brief sends the connection data
 * @param[in] connectionData connection data
 * @return error during execution
 */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_TxPending (ARDISCOVERY_Connection_ConnectionData_t *connectionData);

/**
 * @brief unlocks
 * @param[in] connectionData connection data
 */
static void ARDISCOVERY_Connection_Unlock (ARDISCOVERY_Connection_ConnectionData_t *connectionData);

/*************************
 * Implementation
 *************************/

ARDISCOVERY_Connection_ConnectionData_t* ARDISCOVERY_Connection_New (ARDISCOVERY_Connection_SendJsonCallback_t sendJsonCallback, ARDISCOVERY_Connection_ReceiveJsonCallback_t receiveJsonCallback, void *customData, eARDISCOVERY_ERROR *error)
{
    /* - Create and initialize connection data - */
    
    ARDISCOVERY_Connection_ConnectionData_t *connectionData = NULL;
    eARDISCOVERY_ERROR localError = ARDISCOVERY_OK;
    
    /* Check parameter */
    if ((sendJsonCallback == NULL) || (receiveJsonCallback == NULL))
    {
        localError = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    
    if (localError == ARDISCOVERY_OK)
    {
        connectionData = malloc(sizeof(ARDISCOVERY_Connection_ConnectionData_t));
        if (connectionData != NULL)
        {
            /* Initialize connectionData */
            connectionData->txData.buffer = NULL;
            connectionData->txData.size = 0;
            connectionData->txData.capacity = 0;
            connectionData->rxData.buffer = NULL;
            connectionData->rxData.size = 0;
            connectionData->rxData.capacity = 0;
            connectionData->isAlive = 0;
            ARSAL_Sem_Init (&(connectionData->runningSem), 0, 1);
            connectionData->sendJsoncallback = sendJsonCallback;
            connectionData->receiveJsoncallback = receiveJsonCallback;
            connectionData->customData = customData;
            connectionData->socket = -1;
            memset(&(connectionData->address), 0, sizeof(connectionData->address));
#ifndef _WIN32
            connectionData->abortPipe[0] = -1;
            connectionData->abortPipe[1] = -1;
#else
			connectionData->abortEvent = NULL;
#endif
        }
        else
        {
            localError = ARDISCOVERY_ERROR_ALLOC;
        }
    }
    
    /* Allocate reception buffer */
    if (localError == ARDISCOVERY_OK)
    {
        connectionData->rxData.buffer = (uint8_t *) malloc (sizeof(uint8_t) * ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE);
        if (connectionData->rxData.buffer != NULL)
        {
            connectionData->rxData.size = 0;
            connectionData->rxData.capacity = ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE;
        }
        else
        {
            localError = ARDISCOVERY_ERROR_ALLOC;
        }
    }
    
    /* Allocate transmission buffer */
    if (localError == ARDISCOVERY_OK)
    {
        connectionData->txData.buffer = (uint8_t *) malloc(sizeof(uint8_t) * ARDISCOVERY_CONNECTION_TX_BUFFER_SIZE);
        if (connectionData->txData.buffer != NULL)
        {
            connectionData->txData.size = 0;
            connectionData->txData.capacity = ARDISCOVERY_CONNECTION_TX_BUFFER_SIZE;
        }
        else
        {
            localError = ARDISCOVERY_ERROR_ALLOC;
        }
    }
    
    /* initialize the abortPipe */
    if (localError == ARDISCOVERY_OK)
    {
#ifndef _WIN32
        if (pipe(connectionData->abortPipe) != 0)
#else
		if ((connectionData->abortEvent = CreateEvent(NULL, FALSE, FALSE, NULL)) == NULL)
#endif
        {
            localError = ARDISCOVERY_ERROR_PIPE_INIT;
        }
    }
    
    /* Delete connection data if an error occurred */
    if (localError != ARDISCOVERY_OK)
    {
        ARDISCOVERY_Connection_Delete (&connectionData);
    }

    /* return error */
    if (error != NULL)
    {
        *error = localError;
    }

    return connectionData;
}

eARDISCOVERY_ERROR ARDISCOVERY_Connection_Delete (ARDISCOVERY_Connection_ConnectionData_t **connectionData)
{
    /*
     * Free connection data
     */
     
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    /* Check parameter */
    if (connectionData == NULL)
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
     
    if (error == ARDISCOVERY_OK)
    {
        if ((*connectionData) != NULL)
        {
            if ( ARSAL_Sem_Trywait(&((*connectionData)->runningSem)) == 0)
            {
                /* Destroy runningSem*/
                ARSAL_Sem_Destroy(&((*connectionData)->runningSem));
                
                if ((*connectionData)->txData.buffer)
                {
                    free((*connectionData)->txData.buffer);
                    (*connectionData)->txData.buffer = NULL;
                    (*connectionData)->txData.size = 0;
                    (*connectionData)->txData.capacity = 0;
                }
                if ((*connectionData)->rxData.buffer)
                {
                    free((*connectionData)->rxData.buffer);
                    (*connectionData)->rxData.buffer = NULL;
                    (*connectionData)->rxData.size = 0;
                    (*connectionData)->rxData.capacity = 0;
                }
                
#ifndef _WIN32
                /* close the abortPipe */
                if((*connectionData)->abortPipe[0] != -1)
                {
                    close ((*connectionData)->abortPipe[0]);
                    (*connectionData)->abortPipe[0] = -1;
                }
                
                if((*connectionData)->abortPipe[1] != -1)
                {
                    close ((*connectionData)->abortPipe[1]);
                    (*connectionData)->abortPipe[1] = -1;
                }
#else
				if ((*connectionData)->abortEvent != NULL)
				{
					CloseHandle((*connectionData)->abortEvent);
					(*connectionData)->abortEvent = NULL;
				}
#endif
                
                free (*connectionData);
                (*connectionData) = NULL;
            }
            else
            {
                error = ARDISCOVERY_ERROR_BUSY;
            }
        }
    }
    
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceListeningLoop (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int port)
{
    /*
     * Initialize connection
     */
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    eARDISCOVERY_ERROR loopError = ARDISCOVERY_OK;
    
    int deviceSocket = -1;
    
    /* Check parameter */
    if (connectionData == NULL)
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    
    if (error == ARDISCOVERY_OK)
    {
        /* check if it is already running */
        if (ARSAL_Sem_Trywait(&(connectionData->runningSem)) != 0)
        {
            error = ARDISCOVERY_ERROR_BUSY;
        }
    }
    
    if (error == ARDISCOVERY_OK)
    {
        /* Initialize the server socket */
        error = ARDISCOVERY_Connection_DeviceInitSocket (&deviceSocket, port);
    }
    
    if (error == ARDISCOVERY_OK)
    {
        /*  Initialize is alive */
        connectionData->isAlive = 1;
        
        /* while is alive */
        while (connectionData->isAlive == 1)
        {
            /* Wait for any incoming connection from controller */
            loopError = ARDISCOVERY_Connection_DeviceAccept (connectionData, deviceSocket);
            
            if (loopError == ARDISCOVERY_OK)
            {
                ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "Device accepts a socket");
                
                /* receiption  */
                loopError = ARDISCOVERY_Connection_RxPending (connectionData);
            }
            
            if (loopError == ARDISCOVERY_OK)
            {
                /* sending */
                loopError = ARDISCOVERY_Connection_TxPending (connectionData);
            }
            
            /* close the client socket */
            if (connectionData->socket != -1)
            {
                ARSAL_Socket_Close (connectionData->socket);
                connectionData->socket = -1;
            }
            
            if (loopError != ARDISCOVERY_OK)
            {
                /* print the error occurred */
                ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "error: %s", ARDISCOVERY_Error_ToString (loopError));
            }
        }
        
        /* close deviceSocket */
        if(deviceSocket != -1)
        {
            ARSAL_Socket_Close (deviceSocket);
            deviceSocket = -1;
        }
        
        /* reset the runningSem */
        ARSAL_Sem_Post(&(connectionData->runningSem));
    }
    
    return error;
}

void ARDISCOVERY_Connection_Device_StopListening (ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    /* Check parameter */
    if (connectionData == NULL)
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    
    if (error == ARDISCOVERY_OK)
    {
        /* Stop reception */
        connectionData->isAlive = 0;
        ARDISCOVERY_Connection_Unlock (connectionData);
        
        /* wait the end of the run*/
        ARSAL_Sem_Wait (&(connectionData->runningSem));
        /* reset the runningSem */
        ARSAL_Sem_Post(&(connectionData->runningSem));
    }
}

eARDISCOVERY_ERROR ARDISCOVERY_Connection_ControllerConnection (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int port, const char *ip)
{
    /*
     * Initialize connection
     */
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;

    /* Check parameter */
    if (connectionData == NULL)
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }

    if (error == ARDISCOVERY_OK)
    {
        /* check if it is already running */
        if (ARSAL_Sem_Trywait(&(connectionData->runningSem)) != 0)
        {
            error = ARDISCOVERY_ERROR_BUSY;
        }
    }

    if (error == ARDISCOVERY_OK)
    {
        /* Start by contacting the device we're interested in */
        error = ARDISCOVERY_Connection_ControllerInitSocket (connectionData, port, ip);
        
        if (error == ARDISCOVERY_OK)
        {
            /* sending */
            error = ARDISCOVERY_Connection_TxPending (connectionData);
        }
        
        if (error == ARDISCOVERY_OK)
        {
            /* receiption  */
            error = ARDISCOVERY_Connection_RxPending (connectionData);
        }
        
        /* close the socket*/
        if (connectionData->socket != -1)
        {
            ARSAL_Socket_Close (connectionData->socket);
            connectionData->socket = -1;
        }
        
        /* reset the runningSem */
        ARSAL_Sem_Post(&(connectionData->runningSem));
    }
    return error;
}

/*************************
 * local Implementation
 *************************/

static eARDISCOVERY_ERROR ARDISCOVERY_Connection_CreateSocket (int *socket)
{
    /*
     * Create TCP socket
     */
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    int optval = 1;
    
    *socket = ARSAL_Socket_Create(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (*socket < 0)
    {
        error = ARDISCOVERY_ERROR_SOCKET_CREATION;
    }
    else
    {
        if (setsockopt (*socket, SOL_SOCKET, SO_REUSEADDR, &optval, sizeof optval) != 0)
        {
            error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
        }
    }
    
    return error;
}

static eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceInitSocket (int *deviceSocket, int port)
{
    /*
     * Bind TCP socket for receiving
     * On device, port is self known and listening is done first.
     */
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    struct sockaddr_in recvSin;
    int errorBind, errorListen = 0;

    /* Create TCP socket */
    if (error == ARDISCOVERY_OK)
    {
        error = ARDISCOVERY_Connection_CreateSocket (deviceSocket);
    }

    /* Init socket */
    if (error == ARDISCOVERY_OK)
    {
        /* Server side (device) : listen to anyone */
        recvSin.sin_addr.s_addr = htonl(INADDR_ANY);
        
        recvSin.sin_family = AF_INET;
        recvSin.sin_port = htons(port);

        errorBind = ARSAL_Socket_Bind (*deviceSocket, (struct sockaddr*) &recvSin, sizeof (recvSin));

        if (errorBind != 0)
        {
            ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "bind() failed: %s", strerror(errno));

            switch (errno)
            {
            case EACCES:
                error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                break;

            default:
                error = ARDISCOVERY_ERROR;
                break;
            }
        }

        errorListen = ARSAL_Socket_Listen (*deviceSocket, 10);

        if (errorListen != 0)
        {
            ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "listen() failed: %s", strerror(errno));

            switch (errno)
            {
            case EINVAL:
                error = ARDISCOVERY_ERROR_SOCKET_ALREADY_CONNECTED;
                break;

            default:
                error = ARDISCOVERY_ERROR;
                break;
            }
        }
    }

    return error;
}

static eARDISCOVERY_ERROR ARDISCOVERY_Connection_ControllerInitSocket (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int port, const char *ip)
{
    /*
     * On controller, port is known once received from device.
     */
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    int flags = 0;
    fd_set readSet;
    fd_set writeSet;
    fd_set errorSet;
    int maxFd = 0;
    struct timeval tv = {ARDISCOVERY_CONNECTION_TIMEOUT_SEC, 0};
    int selectErr = 0;
    char dump[10];
    int nbTryToConnect = 0;

    /* Create TCP socket */
    if (error == ARDISCOVERY_OK)
    {
        error = ARDISCOVERY_Connection_CreateSocket (&(connectionData->socket));
    }

    /* Initialize socket */
    if (error == ARDISCOVERY_OK)
    {
        /* Client side (controller) : listen to the device we chose */
        connectionData->address.sin_addr.s_addr = inet_addr (ip);   
        connectionData->address.sin_family = AF_INET;
        connectionData->address.sin_port = htons (port);
        
        /* set the socket non blocking */
#ifndef _WIN32
        flags = fcntl(connectionData->socket, F_GETFL, 0);
        fcntl(connectionData->socket, F_SETFL, flags | O_NONBLOCK);
#else
		unsigned long enable = 1;
		ioctlsocket(connectionData->socket, FIONBIO, &enable);
#endif
        
        ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "contoller try to connect ip:%s port:%d", ip, port);
        
        // try to connect to the socket
        // if host is unreachable (error ARDISCOVERY_ERROR_SOCKET_UNREACHABLE) retry 10 times
        do
        {
            // in that particular case, we retry a connection because the host may be not resolved after a very recent connection
            if (error == ARDISCOVERY_ERROR_SOCKET_UNREACHABLE)
            {
                sleep(ARDISCOVERY_RECONNECTION_TIME_SEC);
            }
            error = ARDISCOVERY_Socket_Connect(connectionData->socket, (struct sockaddr*) &(connectionData->address), sizeof (connectionData->address));
            nbTryToConnect++;
        }
        while ((nbTryToConnect <= ARDISCOVERY_RECONNECTION_NB_RETRY_MAX) && (error == ARDISCOVERY_ERROR_SOCKET_UNREACHABLE));
            
        /*connectError = ARSAL_Socket_Connect (connectionData->socket, (struct sockaddr*) &(connectionData->address), sizeof (connectionData->address));
        
        if (connectError != 0)
        {
            switch (errno)
            {
                case EINPROGRESS:
                    break;
                case EACCES:
                    error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                    break;
                case ENETUNREACH:
                case EHOSTUNREACH:
                {
                    // in that particular case, we retry a connection because the host may be not resolved after a very recent connection
                    ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "connect() failed: %d %s => Try reconnecting after %d seconds", errno, strerror(errno), ARDISCOVERY_RECONNECTION_TIME_SEC);
                    sleep(ARDISCOVERY_RECONNECTION_TIME_SEC);
                    connectError = ARSAL_Socket_Connect (connectionData->socket, (struct sockaddr*) &(connectionData->address), sizeof (connectionData->address));
                    if (connectError != 0)
                    {
                        switch (errno)
                        {
                            case EINPROGRESS:

                                break;
                            case EACCES:
                                error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                                break;
                                
                            default:
                                error = ARDISCOVERY_ERROR;
                                break;
                        }
                    }
                    break;
                }
                default:
                    error = ARDISCOVERY_ERROR;
                    break;
            }
            
            if (error != ARDISCOVERY_OK)
            {
                ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "connect() failed: %d %s", errno, strerror(errno));
            }
        }*/
        
        /* set the socket non blocking */
#ifndef _WIN32
        flags = fcntl(connectionData->socket, F_GETFL, 0);
        fcntl(connectionData->socket, F_SETFL, flags & (~O_NONBLOCK));
#else
		unsigned long enable = 0;
		ioctlsocket(connectionData->socket, FIONBIO, &enable);
#endif
      
		/* this seems to be checking if the socket connected() successfully but we don't have permissions? must be iOS/Android thing */
#ifndef _WIN32 
        /* Initialize set */
        FD_ZERO(&readSet);
        FD_ZERO(&writeSet);
        FD_ZERO(&errorSet);
        FD_SET(connectionData->socket, &writeSet);
        FD_SET(connectionData->abortPipe[0], &readSet);
        FD_SET(connectionData->socket, &errorSet);
        
        /* Get the max fd +1 for select call */
        maxFd = (connectionData->socket > connectionData->abortPipe[0]) ? connectionData->socket +1 : connectionData->abortPipe[0] +1;
        
        /* Wait for either file to be reading for a read */
        selectErr = select (maxFd, &readSet, &writeSet, &errorSet, &tv);
        
        if (selectErr < 0)
        {
            /* Read error */
            error = ARDISCOVERY_ERROR_SELECT;
        }
        else if(selectErr == 0)
        {
            /* timeout error*/
            error = ARDISCOVERY_ERROR_TIMEOUT;
        }
        else
        {
            if (FD_ISSET(connectionData->socket, &errorSet))
            {
                error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
            }
            /* No else: no socket error*/
            
            if (FD_ISSET(connectionData->socket, &writeSet))
            {
                int valopt = 0;
                socklen_t lon;
                int getsockoptRes = 0;
                
                lon = sizeof(int);
                getsockoptRes = getsockopt(connectionData->socket, SOL_SOCKET, SO_ERROR, (void*)(&valopt), &lon);
                if (getsockoptRes < 0)
                {
                    error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                }
                else
                {
                    if (valopt)
                    {
                        error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                    }
                    /* No else: socket successfully connected */
                }
            }
            /* No else: socket not ready */
            
            if (FD_ISSET(connectionData->abortPipe[0], &readSet))
            {
                /* If the abortPipe is ready for a read, dump bytes from it (so it won't be ready next time) */
                read (connectionData->abortPipe[0], &dump, 10);
                error = ARDISCOVERY_ERROR_ABORT;
            }
            /* No else: no timeout */
        }
#endif
    }
    
    return error;
}



static eARDISCOVERY_ERROR ARDISCOVERY_Socket_Connect(int sockfd, const struct sockaddr *addr, socklen_t addrlen)
{
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    int connectError = ARSAL_Socket_Connect (sockfd, addr, addrlen);
    
    if (connectError != 0)
    {
        switch (errno)
        {
            case EINPROGRESS:
                /* in connection */
                /* do nothing */
                break;
            case EACCES:
                error = ARDISCOVERY_ERROR_SOCKET_PERMISSION_DENIED;
                break;
            case ENETUNREACH:
            case EHOSTUNREACH:
                ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "connect() failed: %d %s => Try reconnecting after %d seconds", errno, strerror(errno), ARDISCOVERY_RECONNECTION_TIME_SEC);
                error = ARDISCOVERY_ERROR_SOCKET_UNREACHABLE;
                break;
            default:
                error = ARDISCOVERY_ERROR;
                break;
        }
        
        if (error != ARDISCOVERY_OK)
        {
            ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "connect() failed: %d %s", errno, strerror(errno));
        }
    }
    
    return error;
}

#ifndef _WIN32
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceAccept (ARDISCOVERY_Connection_ConnectionData_t *connectionData, int deviceSocket)
{
    /* - Accept connection - */
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    socklen_t clientLen = sizeof (connectionData->address);
  
    fd_set set;
    int maxFd = 0;
    int selectErr =0;
    char dump[10];
    
    /* Initialize set */
    FD_ZERO(&set);
    FD_SET(deviceSocket, &set);
    FD_SET(connectionData->abortPipe[0], &set);
    
    /* Get the max fd +1 for select call */
    maxFd = (deviceSocket > connectionData->abortPipe[0]) ? deviceSocket +1 : connectionData->abortPipe[0] +1;
    
    ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "Device waits to accept a socket");
            
    /* Wait for either file to be reading for a read */
    selectErr = select (maxFd, &set, NULL, NULL, NULL);
    
    if (selectErr < 0)
    {
        /* Read error */
        error = ARDISCOVERY_ERROR_SELECT;
    }
    else
    {
        if (FD_ISSET(deviceSocket, &set))
        {
            /* Wait for any incoming connection from controller */
            connectionData->socket = ARSAL_Socket_Accept (deviceSocket, (struct sockaddr*) &(connectionData->address), &clientLen);
            if (connectionData->socket < 0)
            {
                ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "accept() failed: %s", strerror(errno));
                error = ARDISCOVERY_ERROR_ACCEPT;
            }
        }
        
        if (FD_ISSET(connectionData->abortPipe[0], &set))
        {
            /* If the abortPipe is ready for a read, dump bytes from it (so it won't be ready next time) */
            read (connectionData->abortPipe[0], &dump, 10);
            error = ARDISCOVERY_ERROR_ABORT;
        }
    }
    
    return error;
}

#else

/* I don't feel like messing around with AcceptEx -- for now you just accept and if it fails/takes forever well too bad */
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_DeviceAccept(ARDISCOVERY_Connection_ConnectionData_t *connectionData, int deviceSocket)
{
	/* - Accept connection - */
	eARDISCOVERY_ERROR error = ARDISCOVERY_OK;

	socklen_t clientLen = sizeof(connectionData->address);

	/* Wait for any incoming connection from controller */
	connectionData->socket = ARSAL_Socket_Accept(deviceSocket, (struct sockaddr*) &(connectionData->address), &clientLen);
	if (connectionData->socket < 0)
	{
		ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_CONNECTION_TAG, "accept() failed: %s", strerror(errno));
		error = ARDISCOVERY_ERROR_ACCEPT;
	}

	return error;
}

#endif


#ifndef _WIN32
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_RxPending (ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
    /* - read connection data - */
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    fd_set set;
    int maxFd = 0;
    struct timeval tv = { ARDISCOVERY_CONNECTION_TIMEOUT_SEC, 0 };
    int selectErr = 0;
    char dump[10];
    
    /* Initialize set */
    FD_ZERO(&set);
    FD_SET(connectionData->socket, &set);
    FD_SET(connectionData->abortPipe[0], &set);
    
    /* Get the max fd +1 for select call */
    maxFd = (connectionData->socket > connectionData->abortPipe[0]) ? connectionData->socket +1 : connectionData->abortPipe[0] +1;
    
    /* Wait for either file to be reading for a read */
    selectErr = select (maxFd, &set, NULL, NULL, &tv);
    
    if (selectErr < 0)
    {
        /* Read error */
        error = ARDISCOVERY_ERROR_SELECT;
    }
    else if(selectErr == 0)
    {
        /* timeout error*/
        error = ARDISCOVERY_ERROR_TIMEOUT;
    }
    else
    {
        if (FD_ISSET(connectionData->socket, &set))
        {
            /* Read content from incoming connection */
            int readSize = 0;
            readSize = ARSAL_Socket_Recv (connectionData->socket, connectionData->rxData.buffer, ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE, 0);
            connectionData->rxData.size += readSize;

            while ((error == ARDISCOVERY_OK) && (readSize == ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE))
            {
                ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "realloc size: %d", (connectionData->rxData.capacity + ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE));
                
                //increase the capacity of the buffer and read the following data from the socket
                uint8_t *newBuffer = realloc (connectionData->rxData.buffer, connectionData->rxData.capacity + ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE);
                if (newBuffer != NULL)
                {
                    // update rxData
                    connectionData->rxData.buffer = newBuffer;
                    connectionData->rxData.capacity += ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE;
                    
                    //read socket
                    readSize = ARSAL_Socket_Recv (connectionData->socket, (connectionData->rxData.buffer + connectionData->rxData.size), ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE, 0);
                    
                    /* update the rxdata size */
                    connectionData->rxData.size += readSize;
                }
                else
                {
                    error = ARDISCOVERY_ERROR_ALLOC;
                }
            }
            
            if (error == ARDISCOVERY_OK)
            {
                ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data read size: %d", connectionData->rxData.size);
                
                if (connectionData->rxData.size > 0)
                {
                    ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data read: %s", connectionData->rxData.buffer);
                    
                    /* receive callback */
                    error = connectionData->receiveJsoncallback (connectionData->rxData.buffer, connectionData->rxData.size, inet_ntoa(connectionData->address.sin_addr), connectionData->customData);
                }
                else
                {
                    error = ARDISCOVERY_ERROR_READ;
                }
            }
        }
        
        if (FD_ISSET(connectionData->abortPipe[0], &set))
        {
            /* If the abortPipe is ready for a read, dump bytes from it (so it won't be ready next time) */
            read (connectionData->abortPipe[0], &dump, 10);
            error = ARDISCOVERY_ERROR_ABORT;
        }
    }
    
    return error;
}

#else

static eARDISCOVERY_ERROR ARDISCOVERY_Connection_RxPending(ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
	/* - read connection data - */

	eARDISCOVERY_ERROR error = ARDISCOVERY_OK;

	OVERLAPPED overlapped;
	ZeroMemory(&overlapped, sizeof(overlapped));

	overlapped.hEvent = WSACreateEvent();

	DWORD recvBytes, flags;
	WSABUF dataBuf;

	dataBuf.buf = connectionData->rxData.buffer;
	dataBuf.len = ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE;

	int res = WSARecv(connectionData->socket, &dataBuf, 1, &recvBytes, &flags, &overlapped, NULL);
	if ((res == SOCKET_ERROR) && (WSA_IO_PENDING != GetLastError()))
	{
		error = ARDISCOVERY_ERROR_READ;
	}
	else
	{
		BOOL wait_result;
		HANDLE waits[2] = { overlapped.hEvent, connectionData->abortEvent };
		res = WaitForMultipleObjects(2, waits, FALSE, ARDISCOVERY_CONNECTION_TIMEOUT_SEC * 1000ul);
		switch (res)
		{
		case WAIT_OBJECT_0:
			wait_result = WSAGetOverlappedResult(connectionData->socket, &overlapped, &recvBytes, FALSE, &flags);
			if (wait_result == FALSE)
			{
				error = ARDISCOVERY_ERROR_READ;
			}
			else
			{
				connectionData->rxData.size += recvBytes;
				while ((error == ARDISCOVERY_OK) && (recvBytes == ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE))
				{
					ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "realloc size: %d", (connectionData->rxData.capacity + ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE));

					//increase the capacity of the buffer and read the following data from the socket
					uint8_t *newBuffer = realloc(connectionData->rxData.buffer, connectionData->rxData.capacity + ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE);
					if (newBuffer != NULL)
					{
						// update rxData
						connectionData->rxData.buffer = newBuffer;
						connectionData->rxData.capacity += ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE;

						//read socket
						recvBytes = ARSAL_Socket_Recv(connectionData->socket, (connectionData->rxData.buffer + connectionData->rxData.size), ARDISCOVERY_CONNECTION_RX_BUFFER_SIZE, 0);

						/* update the rxdata size */
						connectionData->rxData.size += recvBytes;
					}
					else
					{
						error = ARDISCOVERY_ERROR_ALLOC;
					}
				}
				if (error == ARDISCOVERY_OK)
				{
					ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data read size: %d", connectionData->rxData.size);

					if (connectionData->rxData.size > 0)
					{
						ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data read: %s", connectionData->rxData.buffer);

						/* receive callback */
						error = connectionData->receiveJsoncallback(connectionData->rxData.buffer, connectionData->rxData.size, inet_ntoa(connectionData->address.sin_addr), connectionData->customData);
					}
					else
					{
						error = ARDISCOVERY_ERROR_READ;
					}
				}
			}
			WSAResetEvent(overlapped.hEvent);
			break;
		case WAIT_OBJECT_0 + 1:
			error = ARDISCOVERY_ERROR_ABORT;
			/* auto reset event, no need to reset */
			break;
		default:
			error = ARDISCOVERY_ERROR_TIMEOUT;
			break;
		}
	}

	CloseHandle(overlapped.hEvent);

	return error;
}

#endif

#ifndef _WIN32
static eARDISCOVERY_ERROR ARDISCOVERY_Connection_TxPending (ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
    /* - send connection data - */
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ssize_t sendSize = 0;
    fd_set readSet;
    fd_set writeSet;
    int maxFd = 0;
    struct timeval tv = { ARDISCOVERY_CONNECTION_TIMEOUT_SEC, 0 };
    int selectErr = 0;
    char dump[10];
    
    /* initilize set */
    FD_ZERO(&readSet);
    FD_ZERO(&writeSet);
    FD_SET(connectionData->socket, &writeSet);
    FD_SET(connectionData->abortPipe[0], &readSet);
    
    /* Get the max fd +1 for select call */
    maxFd = (connectionData->socket > connectionData->abortPipe[0]) ? connectionData->socket +1 : connectionData->abortPipe[0] +1;
    
    /* sending callback */
    error = connectionData->sendJsoncallback (connectionData->txData.buffer, &(connectionData->txData.size), connectionData->customData);
    
    ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data send size: %d", connectionData->txData.size);
    
    /* check the txData size */
    if ((error == ARDISCOVERY_OK) && (connectionData->txData.size > 0) && (connectionData->txData.size <= ARDISCOVERY_CONNECTION_TX_BUFFER_SIZE))
    {
        ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data send: %s", connectionData->txData.buffer);
        
        /* Wait for either file to be reading for a read */
        selectErr = select (maxFd, &readSet, &writeSet, NULL, &tv);
        
        if (selectErr < 0)
        {
            /* Read error */
            error = ARDISCOVERY_ERROR_SELECT;
        }
        else if(selectErr == 0)
        {
            /* timeout error*/
            error = ARDISCOVERY_ERROR_TIMEOUT;
        }
        else
        {
            if (FD_ISSET(connectionData->socket, &writeSet))
            {
                /* Send txData */
                sendSize = ARSAL_Socket_Send(connectionData->socket, connectionData->txData.buffer, connectionData->txData.size, 0);
                if (sendSize < 0)
                {
                    error = ARDISCOVERY_ERROR_SEND;
                }
            }
            
            if (FD_ISSET(connectionData->abortPipe[0], &readSet))
            {
                /* If the abortPipe is ready for a read, dump bytes from it (so it won't be ready next time) */
                read (connectionData->abortPipe[0], &dump, 10);
                error = ARDISCOVERY_ERROR_ABORT;
            }
        }
    }
    
    return error;
}

#else

static eARDISCOVERY_ERROR ARDISCOVERY_Connection_TxPending(ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
	/* - send connection data - */
	eARDISCOVERY_ERROR error = ARDISCOVERY_OK;

	/* sending callback */
	error = connectionData->sendJsoncallback(connectionData->txData.buffer, &(connectionData->txData.size), connectionData->customData);

	ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data send size: %d", connectionData->txData.size);

	if ((error == ARDISCOVERY_OK) && (connectionData->txData.size > 0) && (connectionData->txData.size <= ARDISCOVERY_CONNECTION_TX_BUFFER_SIZE))
	{
		ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARDISCOVERY_CONNECTION_TAG, "data send: %s", connectionData->txData.buffer);

		OVERLAPPED overlapped;
		overlapped.hEvent = WSACreateEvent();

		DWORD sentBytes, flags;
		WSABUF dataBuf;

		dataBuf.buf = connectionData->txData.buffer;
		dataBuf.len = connectionData->txData.size;

		int res = WSARecv(connectionData->socket, &dataBuf, 1, &sentBytes, &flags, &overlapped, NULL);
		if ((res == SOCKET_ERROR) && (WSA_IO_PENDING != GetLastError()))
		{
			error = ARDISCOVERY_ERROR_SEND;
		}
		else
		{
			BOOL wait_result;
			HANDLE waits[2] = { overlapped.hEvent, connectionData->abortEvent };
			res = WaitForMultipleObjects(2, waits, FALSE, ARDISCOVERY_CONNECTION_TIMEOUT_SEC * 1000ul);
			switch (res)
			{
			case WAIT_OBJECT_0:
				wait_result = WSAGetOverlappedResult(connectionData->socket, &overlapped, &sentBytes, FALSE, &flags);
				if (wait_result == FALSE || sentBytes < 0)
				{
					error = ARDISCOVERY_ERROR_SEND;
				}
				WSAResetEvent(overlapped.hEvent);
				break;
			case WAIT_OBJECT_0 + 1:
				error = ARDISCOVERY_ERROR_ABORT;
				/* auto reset event, no need to reset */
				break;
			default:
				error = ARDISCOVERY_ERROR_TIMEOUT;
				break;
			}
		}

		CloseHandle(overlapped.hEvent);
	}

	return error;
}

#endif

void ARDISCOVERY_Connection_Unlock (ARDISCOVERY_Connection_ConnectionData_t *connectionData)
{
#ifndef _WIN32
    /* - signal - */
    
    char *buff = "x";
    
    if (connectionData->abortPipe[1] != -1)
    {
        write (connectionData->abortPipe[1], buff, 1);
    }
#else
	if (connectionData->abortEvent != NULL)
	{
		SetEvent(connectionData->abortEvent);
	}
#endif
}

void ARDISCOVERY_Connection_getControllerIP (ARDISCOVERY_Connection_ConnectionData_t *connectionData, char* buffer)
{
    strncpy (buffer, inet_ntoa (connectionData->address.sin_addr), 16);
}
