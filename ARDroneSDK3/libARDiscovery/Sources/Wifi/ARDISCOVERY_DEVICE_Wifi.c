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
 * @file ARDISCOVERY_DEVICE_Wifi.c
 * @brief Discovery wifi Device contains the informations of a device discovered
 * @date 02/03/2015
 * @author maxime.maitre@parrot.com
 */

#include <stdlib.h>
#include <json/json.h>
#include <libARSAL/ARSAL_Print.h>
#include <libARNetworkAL/ARNETWORKAL_Manager.h>
#include <libARNetworkAL/ARNETWORKAL_Error.h>
#include <libARDiscovery/ARDISCOVERY_Connection.h>
#include <libARDiscovery/ARDISCOVERY_Discovery.h>
#include <libARDiscovery/ARDISCOVERY_Device.h>

#include "ARDISCOVERY_DEVICE_Wifi.h"

#define ARDISCOVERY_DEVICE_WIFI_TAG "ARDISCOVERY_DEVICE_WIFI"

/*************************
 * Private header
 *************************/

// Bebop
#define BEBOP_DEVICE_TO_CONTROLLER_PORT 43210

#define BEBOP_CONTROLLER_TO_DEVICE_NONACK_ID 10
#define BEBOP_CONTROLLER_TO_DEVICE_ACK_ID 11
#define BEBOP_CONTROLLER_TO_DEVICE_EMERGENCY_ID 12
#define BEBOP_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID 13
#define BEBOP_DEVICE_TO_CONTROLLER_NAVDATA_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 1)
#define BEBOP_DEVICE_TO_CONTROLLER_EVENT_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 2)
#define BEBOP_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 3)

// Jumping Sumo
#define JUMPINGSUMO_DEVICE_TO_CONTROLLER_PORT 43210

#define JUMPINGSUMO_CONTROLLER_TO_DEVICE_NONACK_ID 10
#define JUMPINGSUMO_CONTROLLER_TO_DEVICE_ACK_ID 11
#define JUMPINGSUMO_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID 13
#define JUMPINGSUMO_DEVICE_TO_CONTROLLER_NAVDATA_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 1)
#define JUMPINGSUMO_DEVICE_TO_CONTROLLER_EVENT_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 2)
#define JUMPINGSUMO_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID ((ARNETWORKAL_MANAGER_WIFI_ID_MAX /2) - 3)

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_DiscoveryConnect (ARDISCOVERY_Device_t *device);

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_SendJsonCallback (uint8_t *dataTx, uint32_t *dataTxSize, void *customData);

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_ReceiveJsonCallback (uint8_t *dataRx, uint32_t dataRxSize, char *ip, void *customData);

/*************************
 * Implementation
 *************************/

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_CreateSpecificParameters (ARDISCOVERY_Device_t *device, const char *name, const char *address, int port)
{
    // Initialize wifi specific parameters 
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    
    // Check parameters
    if ((device == NULL) ||
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (error == ARDISCOVERY_OK)
    {
        specificWifiParam = malloc(sizeof(ARDISCOVERY_DEVICE_WIFI_t));
        if (specificWifiParam != NULL)
        {
            device->specificParameters = specificWifiParam;
            specificWifiParam->address = NULL;
            specificWifiParam->dicoveryPort = port;
            specificWifiParam->sendJsonCallback = NULL;
            specificWifiParam->receiveJsonCallback = NULL;
            specificWifiParam->jsonCallbacksCustomData = NULL;
            
            // Parameters sended by discovery Json :
            specificWifiParam->deviceToControllerPort = BEBOP_DEVICE_TO_CONTROLLER_PORT;

            // Parameters received by discovery Json :
            specificWifiParam->controllerToDevicePort = -1;
            specificWifiParam->connectionStatus = ARDISCOVERY_OK;
        }
        else
        {
            error = ARDISCOVERY_ERROR_ALLOC;
        }
    }
        
    if (error == ARDISCOVERY_OK)
    {
        specificWifiParam->address = strdup(address);
        if (specificWifiParam->address == NULL)
        {
            error = ARDISCOVERY_ERROR_ALLOC;
        }
    }
    
    // if an error occurred and it is not a bad parameters error
    if (error != ARDISCOVERY_OK)
    {
        // try to delete SpecificParameters
        ARDISCOVERY_DEVICE_Wifi_DeleteSpecificParameters (device);
    }
    // No else: skipped no error
    
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_DeleteSpecificParameters (ARDISCOVERY_Device_t *device)
{
    // -- Delete SpecificParameters allocated by the wifi initialization --
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL; 
    
    // check parameters
    if ((device == NULL) ||
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (error == ARDISCOVERY_OK)
    {
        // free specific parameter
        if (device->specificParameters != NULL)
        {
            specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
            if (specificWifiParam->address != NULL)
            {
                free (specificWifiParam->address);
                specificWifiParam->address = NULL;
            }
            
            free (device->specificParameters);
            device->specificParameters = NULL;
            specificWifiParam = NULL;
        }
    }
    
    return error;
}

void *ARDISCOVERY_DEVICE_Wifi_GetCopyOfSpecificParameters (ARDISCOVERY_Device_t *device, eARDISCOVERY_ERROR *error)
{
    // -- Copy the specificParameters --
    
    eARDISCOVERY_ERROR localError = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParamToCopy = NULL;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    
    // check parameters
    if ((device == NULL) ||
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE))
    {
        localError = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (localError == ARDISCOVERY_OK)
    {
        // cast device->specificWifiParam
        specificWifiParamToCopy = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
        
        if (specificWifiParamToCopy != NULL)
        {
            // Copy wifi specific parameters 
            specificWifiParam = malloc(sizeof(ARDISCOVERY_DEVICE_WIFI_t));
            if (specificWifiParam != NULL)
            {
                specificWifiParam->address = NULL;
                specificWifiParam->dicoveryPort = specificWifiParamToCopy->dicoveryPort;
                
                specificWifiParam->sendJsonCallback = specificWifiParamToCopy->sendJsonCallback;
                specificWifiParam->receiveJsonCallback = specificWifiParamToCopy->receiveJsonCallback;
                specificWifiParam->jsonCallbacksCustomData = specificWifiParamToCopy->jsonCallbacksCustomData;
                
                // Parameters sended by discovery Json :
                specificWifiParam->deviceToControllerPort = specificWifiParamToCopy->deviceToControllerPort;

                // Parameters received by discovery Json :
                specificWifiParam->controllerToDevicePort = specificWifiParamToCopy->controllerToDevicePort;
                specificWifiParam->connectionStatus = specificWifiParamToCopy->connectionStatus;
            }
            else
            {
                localError = ARDISCOVERY_ERROR_ALLOC;
            }
            
            // Copy address
            if ((localError == ARDISCOVERY_OK) && (specificWifiParamToCopy->address != NULL))
            {
                specificWifiParam->address = strdup (specificWifiParamToCopy->address);
                if (specificWifiParam->address == NULL)
                {
                    localError = ARDISCOVERY_ERROR_ALLOC;
                }
            }
            // No else: skipped by error or no address To Copy.
        }
        // NO Else ; No Specific Wifi Parameters To Copy.
    }
    // No else: skipped by error
    
    // delete the SpecificParameters if an error occurred
    if (localError != ARDISCOVERY_OK)
    {
        ARDISCOVERY_DEVICE_Wifi_DeleteSpecificParameters (device);
    }
    // No else: skipped no error
    
    // return the error
    if (error != NULL)
    {
        *error = localError;
    }
    // No else: error is not returned
    
    return specificWifiParam;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_AddConnectionCallbacks (ARDISCOVERY_Device_t *device, ARDISCOVERY_Device_ConnectionJsonCallback_t sendJsonCallback, ARDISCOVERY_Device_ConnectionJsonCallback_t receiveJsonCallback, void *customData)
{
    // -- Wifi Add Connection Callbacks --
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    
    // check parameters
    if ((device == NULL) ||
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE) ||
        (device->specificParameters == NULL))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (error == ARDISCOVERY_OK)
    {
        // cast device->specificWifiParam
        specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
        
        specificWifiParam->sendJsonCallback = sendJsonCallback;
        specificWifiParam->receiveJsonCallback = receiveJsonCallback;
        specificWifiParam->jsonCallbacksCustomData = customData;
    }
    
    return error;
}

ARNETWORKAL_Manager_t *ARDISCOVERY_DEVICE_Wifi_NewARNetworkAL (ARDISCOVERY_Device_t *device, eARDISCOVERY_ERROR *error, eARNETWORKAL_ERROR *errorAL)
{
    // -- Create a new networlAL adapted to the device --
    
    eARDISCOVERY_ERROR localError = ARDISCOVERY_OK;
    eARNETWORKAL_ERROR localErrorAL = ARNETWORKAL_OK;
    ARNETWORKAL_Manager_t *networkAL = NULL;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    
    // check parameters
    if ((device == NULL) || 
        (device->specificParameters == NULL) ||
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE))
    {
        localError = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (localError == ARDISCOVERY_OK)
    {
        // Cast of device->specificParameters
        specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *) device->specificParameters;
        
        // discovery connection
        localError = ARDISCOVERY_DEVICE_Wifi_DiscoveryConnect (device);
    }
    
    if (localError == ARDISCOVERY_OK)
    {
        // Create the ARNetworkALManager
        networkAL = ARNETWORKAL_Manager_New (&localErrorAL);
    }
    
    if ((localError == ARDISCOVERY_OK) && (localErrorAL == ARNETWORKAL_OK))
    {
        // Initialize the ARNetworkALManager        
        localErrorAL = ARNETWORKAL_Manager_InitWifiNetwork (networkAL, specificWifiParam->address, specificWifiParam->controllerToDevicePort, specificWifiParam->deviceToControllerPort, 1);
        
    }
    
    // set localError to ARDISCOVERY_ERROR is an error AL is occured
    if ((localError == ARDISCOVERY_OK) && (localErrorAL != ARNETWORKAL_OK))
    {
        localError = ARDISCOVERY_ERROR;
    }
    
    // return localErrorAL
    if (errorAL != NULL)
    {
        *errorAL = localErrorAL;
    }
    
    // return localError
    if (error != NULL)
    {
        *error = localError;
    }
    
    // delete networkAL if an error occured
    if ((localError != ARDISCOVERY_OK) && (networkAL != NULL))
    {
        ARDISCOVERY_DEVICE_Wifi_DeleteARNetworkAL (device, &networkAL);
    }
    
    return networkAL;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_DeleteARNetworkAL (ARDISCOVERY_Device_t *device, ARNETWORKAL_Manager_t **networkAL)
{
    // --  Delete a networlAL create by ARDISCOVERY_DEVICE_Wifi_NewARNetworkAL --
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    // check parameters
    if ((device == NULL) || 
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_NSNETSERVICE))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (error == ARDISCOVERY_OK)
    {
        if (networkAL != NULL)
        {
            if ((*networkAL) != NULL)
            {
                ARNETWORKAL_Manager_Unlock((*networkAL));
                
                ARNETWORKAL_Manager_CloseWifiNetwork((*networkAL));
                ARNETWORKAL_Manager_Delete(networkAL);
            }
        }
    }
    
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_InitBebopNetworkCongifuration (ARDISCOVERY_Device_t *device, ARDISCOVERY_NetworkConfiguration_t *networkConfiguration)
{
    // -- Initilize network Configuration adapted to a BebopDrone. --
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    // check parameters
    if ((device == NULL) || 
        (device->productID != ARDISCOVERY_PRODUCT_ARDRONE) ||
        (networkConfiguration == NULL))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    static ARNETWORK_IOBufferParam_t c2dParams[] = {
        /* Non-acknowledged commands. */
        {
            .ID = BEBOP_CONTROLLER_TO_DEVICE_NONACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfRetry = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfCell = 2,
            .dataCopyMaxSize = 128,
            .isOverwriting = 1,
        },
        /* Acknowledged commands. */
        {
            .ID = BEBOP_CONTROLLER_TO_DEVICE_ACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = 500,
            .numberOfRetry = 3,
            .numberOfCell = 20,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Emergency commands. */
        {
            .ID = BEBOP_CONTROLLER_TO_DEVICE_EMERGENCY_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK,
            .sendingWaitTimeMs = 10,
            .ackTimeoutMs = 100,
            .numberOfRetry = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfCell = 1,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Video ACK (Initialized later) */
        {
            .ID = BEBOP_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_UNINITIALIZED,
            .sendingWaitTimeMs = 0,
            .ackTimeoutMs = 0,
            .numberOfRetry = 0,
            .numberOfCell = 0,
            .dataCopyMaxSize = 0,
            .isOverwriting = 0,
        },
    };
    size_t numC2dParams = sizeof(c2dParams) / sizeof(ARNETWORK_IOBufferParam_t);

    static ARNETWORK_IOBufferParam_t d2cParams[] = {
        {
            .ID = BEBOP_DEVICE_TO_CONTROLLER_NAVDATA_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfRetry = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfCell = 20,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        {
            .ID = BEBOP_DEVICE_TO_CONTROLLER_EVENT_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = 500,
            .numberOfRetry = 3,
            .numberOfCell = 20,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Video data (Initialized later) */
        {
            .ID = BEBOP_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_UNINITIALIZED,
            .sendingWaitTimeMs = 0,
            .ackTimeoutMs = 0,
            .numberOfRetry = 0,
            .numberOfCell = 0,
            .dataCopyMaxSize = 0,
            .isOverwriting = 0,
        },
    };
    size_t numD2cParams = sizeof(d2cParams) / sizeof(ARNETWORK_IOBufferParam_t);

    static int commandBufferIds[] = {
        BEBOP_DEVICE_TO_CONTROLLER_NAVDATA_ID,
        BEBOP_DEVICE_TO_CONTROLLER_EVENT_ID,
    };
    
    size_t numOfCommandBufferIds = sizeof(commandBufferIds) / sizeof(int);
    
    if (error == ARDISCOVERY_OK)
    {
        networkConfiguration->controllerLoopIntervalMs = 25;
        
        networkConfiguration->controllerToDeviceNotAckId = BEBOP_CONTROLLER_TO_DEVICE_NONACK_ID;
        networkConfiguration->controllerToDeviceAckId = BEBOP_CONTROLLER_TO_DEVICE_ACK_ID;
        networkConfiguration->controllerToDeviceHightPriority = BEBOP_CONTROLLER_TO_DEVICE_EMERGENCY_ID;
        networkConfiguration->controllerToDeviceARStreamAck = BEBOP_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID;
        networkConfiguration->deviceToControllerNotAckId = BEBOP_DEVICE_TO_CONTROLLER_NAVDATA_ID;
        networkConfiguration->deviceToControllerAckId = BEBOP_DEVICE_TO_CONTROLLER_NAVDATA_ID;
        //int deviceToControllerHightPriority = -1;
        networkConfiguration->deviceToControllerARStreamData = BEBOP_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID;
        
        networkConfiguration->controllerToDeviceParams = c2dParams;
        networkConfiguration->numberOfControllerToDeviceParam = numC2dParams;
        
        networkConfiguration->deviceToControllerParams = d2cParams;
        networkConfiguration->numberOfDeviceToControllerParam = numD2cParams;
        
        networkConfiguration->pingDelayMs = 0;
        
        networkConfiguration->numberOfDeviceToControllerCommandsBufferIds = numOfCommandBufferIds;
        networkConfiguration->deviceToControllerCommandsBufferIds = commandBufferIds;
    }
    
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_InitJumpingSumoNetworkCongifuration (ARDISCOVERY_Device_t *device, ARDISCOVERY_NetworkConfiguration_t *networkConfiguration)
{
    // -- Initilize network Configuration adapted to a Jumping Sumo. --
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    // check parameters
    if ((device == NULL) || 
        (device->productID != ARDISCOVERY_PRODUCT_JS) ||
        (networkConfiguration == NULL))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    static ARNETWORK_IOBufferParam_t c2dParams[] = {
        /* Non-acknowledged commands. */
        {
            .ID = JUMPINGSUMO_CONTROLLER_TO_DEVICE_NONACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA,
            .sendingWaitTimeMs = 5,
            .ackTimeoutMs = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfRetry = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfCell = 10,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Acknowledged commands. */
        {
            .ID = JUMPINGSUMO_CONTROLLER_TO_DEVICE_ACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = 500,
            .numberOfRetry = 3,
            .numberOfCell = 20,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Video ACK (Initialized later) */
        {
            .ID = JUMPINGSUMO_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_UNINITIALIZED,
            .sendingWaitTimeMs = 0,
            .ackTimeoutMs = 0,
            .numberOfRetry = 0,
            .numberOfCell = 0,
            .dataCopyMaxSize = 0,
            .isOverwriting = 0,
        },
    };
    size_t numC2dParams = sizeof(c2dParams) / sizeof(ARNETWORK_IOBufferParam_t);

    static ARNETWORK_IOBufferParam_t d2cParams[] = {
        {
            .ID = JUMPINGSUMO_DEVICE_TO_CONTROLLER_NAVDATA_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfRetry = ARNETWORK_IOBUFFERPARAM_INFINITE_NUMBER,
            .numberOfCell = 10,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        {
            .ID = JUMPINGSUMO_DEVICE_TO_CONTROLLER_EVENT_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK,
            .sendingWaitTimeMs = 20,
            .ackTimeoutMs = 500,
            .numberOfRetry = 3,
            .numberOfCell = 20,
            .dataCopyMaxSize = 128,
            .isOverwriting = 0,
        },
        /* Video data (Initialized later) */
        {
            .ID = JUMPINGSUMO_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID,
            .dataType = ARNETWORKAL_FRAME_TYPE_UNINITIALIZED,
            .sendingWaitTimeMs = 0,
            .ackTimeoutMs = 0,
            .numberOfRetry = 0,
            .numberOfCell = 0,
            .dataCopyMaxSize = 0,
            .isOverwriting = 0,
        },
    };
    size_t numD2cParams = sizeof(d2cParams) / sizeof(ARNETWORK_IOBufferParam_t);

    static int commandBufferIds[] = {
        JUMPINGSUMO_DEVICE_TO_CONTROLLER_NAVDATA_ID,
        JUMPINGSUMO_DEVICE_TO_CONTROLLER_EVENT_ID,
    };
    
    size_t numOfCommandBufferIds = sizeof(commandBufferIds) / sizeof(int);
    
    if (error == ARDISCOVERY_OK)
    {
        networkConfiguration->controllerLoopIntervalMs = 50;
        
        networkConfiguration->controllerToDeviceNotAckId = JUMPINGSUMO_CONTROLLER_TO_DEVICE_NONACK_ID;
        networkConfiguration->controllerToDeviceAckId = JUMPINGSUMO_CONTROLLER_TO_DEVICE_ACK_ID;
        networkConfiguration->controllerToDeviceHightPriority = -1;
        networkConfiguration->controllerToDeviceARStreamAck = JUMPINGSUMO_CONTROLLER_TO_DEVICE_VIDEO_ACK_ID;
        networkConfiguration->deviceToControllerNotAckId = JUMPINGSUMO_DEVICE_TO_CONTROLLER_NAVDATA_ID;
        networkConfiguration->deviceToControllerAckId = JUMPINGSUMO_DEVICE_TO_CONTROLLER_NAVDATA_ID;
        //int deviceToControllerHightPriority = -1;
        networkConfiguration->deviceToControllerARStreamData = JUMPINGSUMO_DEVICE_TO_CONTROLLER_VIDEO_DATA_ID;
        
        networkConfiguration->controllerToDeviceParams = c2dParams;
        networkConfiguration->numberOfControllerToDeviceParam = numC2dParams;
        
        networkConfiguration->deviceToControllerParams = d2cParams;
        networkConfiguration->numberOfDeviceToControllerParam = numD2cParams;
        
        networkConfiguration->pingDelayMs = 0;
        
        networkConfiguration->numberOfDeviceToControllerCommandsBufferIds = numOfCommandBufferIds;
        networkConfiguration->deviceToControllerCommandsBufferIds = commandBufferIds;
    }
    
    return error;
}


 /*************************
 * local Implementation
 *************************/

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_DiscoveryConnect (ARDISCOVERY_Device_t *device)
{
    
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    ARDISCOVERY_Connection_ConnectionData_t *discoveryData = NULL;
    
    // check parameters
    if ((device == NULL) || 
        (ARDISCOVERY_getProductService (device->productID) != ARDISCOVERY_PRODUCT_ARDRONE) ||
        (device->specificParameters == NULL))
    {
        error = ARDISCOVERY_ERROR_BAD_PARAMETER;
    }
    // No Else: the checking parameters sets error to ARNETWORK_ERROR_BAD_PARAMETER and stop the processing
    
    if (error == ARDISCOVERY_OK)
    {
        // Cast of device->specificParameters
        specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
        
        // New Discovery Connection
        discoveryData = ARDISCOVERY_Connection_New (ARDISCOVERY_DEVICE_Wifi_SendJsonCallback, ARDISCOVERY_DEVICE_Wifi_ReceiveJsonCallback, device, &error);
    }
    
    if (error == ARDISCOVERY_OK)
    {        
        error = ARDISCOVERY_Connection_ControllerConnection (discoveryData, specificWifiParam->dicoveryPort, specificWifiParam->address);
    }
    
    // Cleanup
    ARDISCOVERY_Connection_Delete(&discoveryData);
        
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_SendJsonCallback (uint8_t *dataTx, uint32_t *dataTxSize, void *customData)
{
    // -- Connection callback to send the Json --
    
    // local declarations
    ARDISCOVERY_Device_t *device = (ARDISCOVERY_Device_t *)customData;
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    
    int jsonSize = 0;
    json_object *jsonObj = NULL;
    json_object *valueJsonObj = NULL;
    
    if ((dataTx == NULL) ||
        (dataTxSize == NULL) ||
        (device == NULL) ||
        (device->specificParameters == NULL))
    {
        error = ARDISCOVERY_ERROR; //TODO see if set bad parameter
    }
    
    if (error == ARDISCOVERY_OK)
    {
        // Cast of device->specificParameters
        specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
        
        jsonObj = json_object_new_object ();
        
        // add ARDISCOVERY_CONNECTION_JSON_D2CPORT_KEY
        valueJsonObj = json_object_new_int (specificWifiParam->deviceToControllerPort);
        json_object_object_add (jsonObj, ARDISCOVERY_CONNECTION_JSON_D2CPORT_KEY, valueJsonObj);
                
        // sending Json callback 
        if (specificWifiParam->sendJsonCallback != NULL)
        {            
            error = specificWifiParam->sendJsonCallback (jsonObj, specificWifiParam->jsonCallbacksCustomData);
        }
    }
    
    if (error == ARDISCOVERY_OK)
    {
        // copy json in dataTx
        jsonSize = strlen(json_object_to_json_string (jsonObj));
        if (jsonSize <= ARDISCOVERY_CONNECTION_TX_BUFFER_SIZE)
        {
            memcpy (dataTx, json_object_to_json_string (jsonObj), jsonSize);
            *dataTxSize = jsonSize;
            
        }
        else
        {
            error = ARDISCOVERY_ERROR_JSON_BUFFER_SIZE;
        }
    }

    if (jsonObj != NULL)
    {
        /* free json object */
        json_object_put (jsonObj);
        jsonObj = NULL; 
    }

    
    return error;
}

eARDISCOVERY_ERROR ARDISCOVERY_DEVICE_Wifi_ReceiveJsonCallback (uint8_t *dataRx, uint32_t dataRxSize, char *ip, void *customData)
{
    // -- Connection callback to receive the Json --
    
    // local declarations
    ARDISCOVERY_Device_t *device = (ARDISCOVERY_Device_t *)customData;
    ARDISCOVERY_DEVICE_WIFI_t *specificWifiParam = NULL;
    eARDISCOVERY_ERROR error = ARDISCOVERY_OK;
    
    json_object *jsonObj = NULL;
    json_object *valueJsonObj = NULL;
    
    if ((dataRx == NULL) ||
        (dataRxSize == 0) ||
        (device == NULL) ||
        (device->specificParameters == NULL))
    {
        error = ARDISCOVERY_ERROR;
    }
    
    if (error == ARDISCOVERY_OK)
    {        
        // Cast of device->specificParameters
        specificWifiParam = (ARDISCOVERY_DEVICE_WIFI_t *)device->specificParameters;
        
        // parssing of the json 
        jsonObj = json_tokener_parse ((char *)dataRx);
        if (is_error (jsonObj))
        {
            error = ARDISCOVERY_ERROR_JSON_PARSSING;
        }
    }
    
    if (error == ARDISCOVERY_OK)
    {
        // get ARDISCOVERY_CONNECTION_JSON_C2DPORT_KEY
        valueJsonObj = json_object_object_get (jsonObj, ARDISCOVERY_CONNECTION_JSON_C2DPORT_KEY);
        if (valueJsonObj != NULL)
        {
            specificWifiParam->controllerToDevicePort = json_object_get_int(valueJsonObj);
        }
        
        // get ARDISCOVERY_CONNECTION_JSON_STATUS_KEY

        valueJsonObj = json_object_object_get (jsonObj, ARDISCOVERY_CONNECTION_JSON_STATUS_KEY);
        if (valueJsonObj != NULL)
        {
            specificWifiParam->connectionStatus = json_object_get_int(valueJsonObj);
        }
    }
    
    if (error == ARDISCOVERY_OK)
    {
        // receiving Json callback 
        if (specificWifiParam->receiveJsonCallback != NULL)
        {
            specificWifiParam->receiveJsonCallback (jsonObj, specificWifiParam->jsonCallbacksCustomData);
        }
    }
    
    if (jsonObj != NULL)
    {
        /* free json object */
        json_object_put (jsonObj);
        jsonObj = NULL; 
    }
    
    return error;
}

