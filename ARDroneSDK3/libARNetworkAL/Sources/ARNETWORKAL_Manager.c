/*
    Copyright (C) 2014 Parrot SA

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
/**
 * @file ARNETWORKAL_Manager.c
 * @brief network manager allow to send over network.
 * @date 25/04/2013
 * @author frederic.dhaeyer@parrot.com
 */

/*****************************************
 *
 *             include file :
 *
 *****************************************/
#include <config.h>
#include <stdlib.h>

#include <inttypes.h>
#include <stddef.h>
#include <string.h>
#include <errno.h>

#include <libARSAL/ARSAL_Print.h>

#include <libARNetworkAL/ARNETWORKAL_Manager.h>
#include <libARNetworkAL/ARNETWORKAL_Error.h>
#include "Wifi/ARNETWORKAL_WifiNetwork.h"

#if defined(HAVE_COREBLUETOOTH_COREBLUETOOTH_H)
#include "BLE/ARNETWORKAL_BLENetwork.h"
#endif

#include "ARNETWORKAL_Manager.h"

/*****************************************
 *
 *             define :
 *
 *****************************************/

#define ARNETWORKAL_MANAGER_TAG "ARNETWORKAL_Manager"

/*****************************************
 *
 *             private header:
 *
 *****************************************/


/*****************************************
 *
 *             implementation :
 *
 *****************************************/
ARNETWORKAL_Manager_t* ARNETWORKAL_Manager_New (eARNETWORKAL_ERROR *error)
{
    /** -- Create a new Manager -- */

    /** local declarations */
    ARNETWORKAL_Manager_t *manager = NULL;
    eARNETWORKAL_ERROR localError = ARNETWORKAL_OK;
    /** Create the Manager */
    manager = malloc (sizeof(ARNETWORKAL_Manager_t));
    if (manager != NULL)
    {
        /** Initialize to default values */
        manager->pushFrame = (ARNETWORKAL_Manager_PushFrame_t) NULL;
        manager->popFrame = (ARNETWORKAL_Manager_PopFrame_t) NULL;
        manager->send = (ARNETWORKAL_Manager_Send_t) NULL;
        manager->receive = (ARNETWORKAL_Manager_Receive_t) NULL;
        manager->unlock = (ARNETWORKAL_Manager_Unlock_t) NULL;
        manager->getBandwidth = (ARNETWORKAL_Manager_GetBandwidth_t) NULL;
        manager->setOnDisconnectCallback = (ARNETWORKAL_Manager_SetOnDisconnectCallback_t) NULL;
        manager->bandwidthThread = (ARNETWORKAL_Manager_BandwidthThread_t) NULL;
        manager->receiverObject = (void *) NULL;
        manager->senderObject = (void *) NULL;
        manager->maxIds = ARNETWORKAL_MANAGER_DEFAULT_ID_MAX;
        manager->maxBufferSize = 0;
    }
    else
    {
        localError = ARNETWORKAL_ERROR_ALLOC;
    }

    /** delete the Manager if an error occurred */
    if (localError != ARNETWORKAL_OK)
    {
        ARSAL_PRINT (ARSAL_PRINT_ERROR, ARNETWORKAL_MANAGER_TAG, "error: %d occurred \n", localError);
        ARNETWORKAL_Manager_Delete (&manager);
    }

    /** return the error */
    if (error != NULL)
    {
        *error = localError;
    }

    return manager;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_InitWifiNetwork (ARNETWORKAL_Manager_t *manager, const char *addr, int sendingPort, int receivingPort, int recvTimeoutSec)
{
    /** -- Initialize the Wifi Network -- */

    /** local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;

    /** check paratemters*/
    if ((manager == NULL) || (addr == NULL))
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }

    if(error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_WifiNetwork_New(manager);
    }

    if (error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_WifiNetwork_Connect (manager, addr, sendingPort);
    }

    if (error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_WifiNetwork_Bind (manager, receivingPort, recvTimeoutSec);
    }

    if(error == ARNETWORKAL_OK)
    {
        manager->pushFrame = ARNETWORKAL_WifiNetwork_PushFrame;
        manager->popFrame = ARNETWORKAL_WifiNetwork_PopFrame;
        manager->send = ARNETWORKAL_WifiNetwork_Send;
        manager->receive = ARNETWORKAL_WifiNetwork_Receive;
        manager->unlock = ARNETWORKAL_WifiNetwork_Signal;
        manager->getBandwidth = ARNETWORKAL_WifiNetwork_GetBandwidth;
        manager->bandwidthThread = ARNETWORKAL_WifiNetwork_BandwidthThread;
        manager->maxIds = ARNETWORKAL_MANAGER_WIFI_ID_MAX;
        manager->maxBufferSize = ARNETWORKAL_WIFINETWORK_MAX_DATA_BUFFER_SIZE;
        manager->setOnDisconnectCallback = &ARNETWORKAL_WifiNetwork_SetOnDisconnectCallback;
    }
    else
    {
        ARNETWORKAL_WifiNetwork_Delete(manager);
    }

    return error;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_CancelWifiNetwork (ARNETWORKAL_Manager_t *manager)
{
    /* -- Cancel the initWifiNetwork -- */
    
    /* local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;
    
    if (manager == NULL)
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }
    
    if (error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_WifiNetwork_Cancel(manager);
    }
    
    return error;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_CloseWifiNetwork (ARNETWORKAL_Manager_t *manager)
{
    /* -- Close the Wifi Network -- */

    /* local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;

    if(manager == NULL)
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }

    if(error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_WifiNetwork_Delete(manager);
    }

    return error;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_InitBLENetwork (ARNETWORKAL_Manager_t *manager, ARNETWORKAL_BLEDeviceManager_t deviceManager, ARNETWORKAL_BLEDevice_t device, int recvTimeoutSec, int *notificationIDs, int numberOfNotificationID)
{
    /* local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;

#if defined(HAVE_COREBLUETOOTH_COREBLUETOOTH_H)
    /* -- Initialize the BLE Network -- */
    /* check parameters*/
    if (manager == NULL)
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }

    if(error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_BLENetwork_New(manager);
    }

    if (error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_BLENetwork_Connect(manager, deviceManager, device, recvTimeoutSec, notificationIDs, numberOfNotificationID);
    }

    if(error == ARNETWORKAL_OK)
    {
        manager->pushFrame = ARNETWORKAL_BLENetwork_PushFrame;
        manager->popFrame = ARNETWORKAL_BLENetwork_PopFrame;
        manager->send = ARNETWORKAL_BLENetwork_Send;
        manager->receive = ARNETWORKAL_BLENetwork_Receive;
        manager->unlock = ARNETWORKAL_BLENetwork_Unlock;
        manager->getBandwidth = ARNETWORKAL_BLENetwork_GetBandwidth;
        manager->bandwidthThread = ARNETWORKAL_BLENetwork_BandwidthThread;
        manager->maxIds = ARNETWORKAL_MANAGER_BLE_ID_MAX;
        manager->maxBufferSize = ARNETWORKAL_BLENETWORK_MAX_BUFFER_SIZE;
        manager->setOnDisconnectCallback = ARNETWORKAL_BLENetwork_SetOnDisconnectCallback;
    }
    else
    {
        error = ARNETWORKAL_BLENetwork_Delete(manager);
    }

#else
    error = ARNETWORKAL_ERROR_NETWORK_TYPE;
#endif

    return error;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_CancelBLENetwork (ARNETWORKAL_Manager_t *manager)
{
    /* Cancel initBLENetwork */
    /* local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;
    
#if defined(HAVE_COREBLUETOOTH_COREBLUETOOTH_H)
    /* check parameters*/
    if (manager == NULL)
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }
    
    if( error == ARNETWORKAL_OK)
    {
        error = ARNETWORKAL_BLENetwork_Cancel(manager);
    }
#else
    error = ARNETWORKAL_ERROR_NETWORK_TYPE;
#endif
    
    return error;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_CloseBLENetwork (ARNETWORKAL_Manager_t *manager)
{
    /** local declarations */
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;

#if defined(HAVE_COREBLUETOOTH_COREBLUETOOTH_H)
    /** -- Close the BLE Network -- */
    if(manager)
    {
        error = ARNETWORKAL_BLENetwork_Delete(manager);
    }
#else
    error = ARNETWORKAL_ERROR_NETWORK_TYPE;
#endif

    return error;
}

void ARNETWORKAL_Manager_Delete (ARNETWORKAL_Manager_t **manager)
{
    /** -- Delete the Manager -- */

    if (manager != NULL)
    {
        if ((*manager) != NULL)
        {
            free (*manager);
            (*manager) = NULL;
        }
    }
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_Unlock (ARNETWORKAL_Manager_t *manager)
{
    eARNETWORKAL_ERROR err = ARNETWORKAL_OK;
    if (manager == NULL)
    {
        err = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }
    else if (manager->unlock == NULL)
    {
        err = ARNETWORKAL_ERROR_MANAGER_OPERATION_NOT_SUPPORTED;
    }
    else
    {
        err = manager->unlock(manager);
    }
    return err;
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_GetBandwidth (ARNETWORKAL_Manager_t *manager, uint32_t *uploadBw, uint32_t *downloadBw)
{
    eARNETWORKAL_ERROR err = ARNETWORKAL_OK;
    if (manager == NULL)
    {
        err = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }
    else if (manager->getBandwidth == NULL)
    {
        err = ARNETWORKAL_ERROR_MANAGER_OPERATION_NOT_SUPPORTED;
    }
    else
    {
        err = manager->getBandwidth (manager, uploadBw, downloadBw);
    }
    return err;
}

void* ARNETWORKAL_Manager_BandwidthThread (void *manager)
{
    if (manager == NULL)
    {
        return (void *)0;
    }
    ARNETWORKAL_Manager_t *trueManager = (ARNETWORKAL_Manager_t *)manager;
    if (trueManager->bandwidthThread == NULL)
    {
        return (void *)0;
    }
    return trueManager->bandwidthThread (manager);
}

eARNETWORKAL_ERROR ARNETWORKAL_Manager_SetOnDisconnectCallback (ARNETWORKAL_Manager_t *manager, ARNETWORKAL_Manager_OnDisconnect_t onDisconnectCallback, void *customData)
{
    /* -- set the OnDisconnect Callback -- */
    
    eARNETWORKAL_ERROR error = ARNETWORKAL_OK;
    
    if (manager == NULL)
    {
        error = ARNETWORKAL_ERROR_BAD_PARAMETER;
    }
    else if (manager->setOnDisconnectCallback == NULL)
    {
        error = ARNETWORKAL_ERROR_MANAGER_OPERATION_NOT_SUPPORTED;
    }
    else
    {
        error = manager->setOnDisconnectCallback (manager, onDisconnectCallback, customData);
    }
    
    return error;
}
