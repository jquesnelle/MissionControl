/*
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
/**
* @file libARSAL/ARSAL_Mutex_Win32.c
* @brief This file contains sources about mutex abstraction layer for Windows
* @date 08/11/2015
* @author jquesnelle@gmail.com
*/

#include <stdlib.h>
#include "config.h"
#include <libARSAL/ARSAL_Mutex.h>
#include <libARSAL/ARSAL_Time.h>
#include <libARSAL/ARSAL_Print.h>
#include <libARSAL/ARSAL_Error.h>

#define ARSAL_MUTEX_TAG "ARSAL_Mutex"

int ARSAL_Mutex_Init(ARSAL_Mutex_t *mutex)
{
	if (mutex)
	{
		CRITICAL_SECTION* pcs = (CRITICAL_SECTION*)malloc(sizeof(CRITICAL_SECTION));
		InitializeCriticalSection(pcs);
		*mutex = pcs;
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;	
}

int ARSAL_Mutex_Destroy(ARSAL_Mutex_t *mutex)
{
	if (mutex && *mutex)
	{
		DeleteCriticalSection((CRITICAL_SECTION*)*mutex);
		free(*mutex);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}


int ARSAL_Mutex_Lock(ARSAL_Mutex_t *mutex)
{
	if (mutex && *mutex)
	{
		EnterCriticalSection((CRITICAL_SECTION*)*mutex);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Mutex_Trylock(ARSAL_Mutex_t *mutex)
{
	if (mutex && *mutex)
	{
		CRITICAL_SECTION* pcs = *mutex;
		DWORD ret = TryEnterCriticalSection(pcs);
		return ret != 0 ? 0 : EBUSY;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Mutex_Unlock(ARSAL_Mutex_t *mutex)
{
	if (mutex && *mutex)
	{
		LeaveCriticalSection((CRITICAL_SECTION*)*mutex);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Init(ARSAL_Cond_t *cond)
{
	if (cond)
	{
		CONDITION_VARIABLE* pcv = (CONDITION_VARIABLE*)malloc(sizeof(CONDITION_VARIABLE));
		InitializeConditionVariable(pcv);
		*cond = pcv;
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Destroy(ARSAL_Cond_t *cond)
{
	if (cond)
	{
		//no DeleteConditionVariable?
		free(*cond);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Wait(ARSAL_Cond_t *cond, ARSAL_Mutex_t *mutex)
{
	if (cond && *cond && mutex && *mutex)
	{
		return SleepConditionVariableCS((CONDITION_VARIABLE*)*cond, (CRITICAL_SECTION*)*mutex, INFINITE) ? 0 : GetLastError();
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Timedwait(ARSAL_Cond_t *cond, ARSAL_Mutex_t *mutex, int timeout)
{
	if (cond && *cond && mutex && *mutex)
	{
		if (SleepConditionVariableCS((CONDITION_VARIABLE*)*cond, (CRITICAL_SECTION*)*mutex, INFINITE))
			return 0;
		DWORD result = GetLastError();
		return result == ERROR_TIMEOUT ? ETIMEDOUT : result;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Signal(ARSAL_Cond_t *cond)
{
	if (cond && *cond)
	{
		WakeConditionVariable((CONDITION_VARIABLE*)*cond);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Cond_Broadcast(ARSAL_Cond_t *cond)
{
	if (cond && *cond)
	{
		WakeAllConditionVariable((CONDITION_VARIABLE*)*cond);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}