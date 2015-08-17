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
* @file libARSAL/ARSAL_Thread_Win32.c
* @brief This file contains sources about thread abstraction layer for Windows
* @date 08/12/2015
* @author jquesnelle@gmail.com
*/

#include <libARSAL/ARSAL_Thread.h>
#include <libARSAL/ARSAL_Print.h>
#include <libARSAL/ARSAL_Error.h>
#include "Windows.h"

typedef struct win32_thread_info_t {
	HANDLE handle;
	ARSAL_Thread_Routine_t start;
	void* arg;
	void* ret;
} win32_thread_info_t;

static DWORD WINAPI ThreadProc(LPVOID lpParameter)
{
	win32_thread_info_t* info = (win32_thread_info_t*)lpParameter;
	info->ret = info->start(info->arg);
	return 0;
}

int ARSAL_Thread_Create(ARSAL_Thread_t *thread, ARSAL_Thread_Routine_t routine, void *arg)
{
	if (thread && routine)
	{
		win32_thread_info_t* info = (win32_thread_info_t*)malloc(sizeof(win32_thread_info_t));
		info->arg = arg;
		info->start = routine;
		info->handle = CreateThread(NULL, 0, ThreadProc, info, 0, NULL);
		*thread = info;
		return info->handle ? 0 : GetLastError();
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Thread_Join(ARSAL_Thread_t thread, void **retval)
{
	if (thread)
	{
		if (WaitForSingleObject(thread, INFINITE) == WAIT_OBJECT_0)
		{
			if (retval)
			{
				win32_thread_info_t* info = (win32_thread_info_t*)thread;
				*retval = info->ret;
			}
				
			return 0;
		}
		return GetLastError();
	}
	return ARSAL_ERROR_BAD_PARAMETER;
}

int ARSAL_Thread_Destroy(ARSAL_Thread_t *thread)
{
	/* Code currently does some weird stuff with thread. For example in ARCONTROLLER_Device_Start it calls
	* ARSAL_Thread_Destroy immediately after the thread starts. Fine for pthread but we need *thread to exist
	* throughout the limetime of the thread (makes sense right?). For now we'll just have to leak sizeof(win32_thread_info_t)
	* until I can rearchitecture it.
	*/
#if 0
	if (*thread)
	{
		win32_thread_info_t* info = (win32_thread_info_t*)*thread;
		if (info->handle)
		{
			CloseHandle(info->handle);
		}
		free(info);
		return 0;
	}
	return ARSAL_ERROR_BAD_PARAMETER;
#endif
	return 0;
}