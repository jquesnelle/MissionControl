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

/**
* @file ARSAL_MD5_WinCrpyto.c
* @brief MD5 WinCrypt backend for MD5 Manager.
* @date 08/11/2015
* @author jquesnelle@gmail.com
*/

#include "config.h"
#include <stdio.h>
#include <stdint.h>
#include "libARSAL/ARSAL_Error.h"
#include "libARSAL/ARSAL_Print.h"
#include "libARSAL/ARSAL_MD5_Manager.h"
#include <Wincrypt.h>

#define ARUTILS_MD5_TAG         "MD5_WinCrypt"

/* Exported functions prototypes. */
eARSAL_ERROR ARSAL_MD5_Manager_Init(ARSAL_MD5_Manager_t *manager);
void ARSAL_MD5_Manager_Close(ARSAL_MD5_Manager_t *manager);

/* Local function prototypes */
static eARSAL_ERROR ARSAL_MD5_Check(void *md5Object, const char *filePath, const char *md5Txt);
static eARSAL_ERROR ARSAL_MD5_Compute(void *md5Object, const char *filePath, uint8_t *md5, int md5Len);
static eARSAL_ERROR ARSAL_MD5_GetMd5AsTxt(const uint8_t *md5, int md5Len, char *md5Txt, int md5TxtLen);

eARSAL_ERROR ARSAL_MD5_Manager_Init(ARSAL_MD5_Manager_t *manager)
{
	eARSAL_ERROR result = ARSAL_OK;

	if (manager == NULL)
	{
		result = ARSAL_ERROR_BAD_PARAMETER;
	}

	if (result == ARSAL_OK)
	{
		HCRYPTPROV hCryptProv = NULL;
		if (CryptAcquireContext(&hCryptProv, NULL, NULL, PROV_RSA_FULL, 0))
		{
			manager->md5Object = (void*)hCryptProv;
		}
		else
		{
			result = ARSAL_ERROR_ALLOC;
		}
	}

	if (result == ARSAL_OK)
	{
		manager->md5Check = ARSAL_MD5_Check;
		manager->md5Compute = ARSAL_MD5_Compute;
		manager->md5Object = NULL;
	}

	return result;
}

void ARSAL_MD5_Manager_Close(ARSAL_MD5_Manager_t *manager)
{
	if (manager != NULL)
	{
		if (manager->md5Object != NULL)
		{
			CryptReleaseContext((HCRYPTPROV)manager->md5Object, 0);
		}
		manager->md5Check = NULL;
		manager->md5Compute = NULL;
		manager->md5Object = NULL;
	}
}

static eARSAL_ERROR ARSAL_MD5_Check(void *md5Object, const char *filePath, const char *md5Txt)
{
	eARSAL_ERROR result = ARSAL_OK;
	uint8_t digest[32];
	char md5Src[(32 * 2) + 1];

	if ((filePath == NULL) || (md5Txt == NULL))
	{
		result = ARSAL_ERROR_BAD_PARAMETER;
	}

	if (result == ARSAL_OK)
	{
		result = ARSAL_MD5_Compute(md5Object, filePath, digest, sizeof(digest));
	}

	if (result == ARSAL_OK)
	{
		result = ARSAL_MD5_GetMd5AsTxt(digest, sizeof(digest), md5Src, sizeof(md5Src));
	}

	if (result == ARSAL_OK)
	{
		if (strcmp(md5Txt, md5Src) != 0)
		{
			ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARUTILS_MD5_TAG, "MD5 mismatch: src=%s dst=%s", md5Src, md5Txt);
			result = ARSAL_ERROR_MD5;
		}
		else
		{
			ARSAL_PRINT(ARSAL_PRINT_DEBUG, ARUTILS_MD5_TAG, "MD5 match: src=dst=%s", md5Src);
		}
	}

	return result;
}

static eARSAL_ERROR ARSAL_MD5_Compute(void *md5Object, const char *filePath, uint8_t *md5, int md5Len)
{
	eARSAL_ERROR result = ARSAL_OK;
	HCRYPTHASH hHash;
	size_t digest_size;
	FILE *file;
	size_t count;
	uint8_t *block = NULL;
	long blocksize;

	if ((filePath == NULL) || (md5 == NULL) || (md5Object == NULL))
	{
		result = ARSAL_ERROR_BAD_PARAMETER;
	}

	if (result == ARSAL_OK)
	{
		if(!CryptCreateHash((HCRYPTPROV)md5Object, CALG_MD5, 0, 0, &hHash))
		{
			result = ARSAL_ERROR_ALLOC;
		}
	}

	if (result == ARSAL_OK)
	{
		digest_size = 32;
		if (md5Len < digest_size)
		{
			result = ARSAL_ERROR_BAD_PARAMETER;
		}
	}

	/* Use page size as block size. */
	if (result == ARSAL_OK)
	{
		blocksize = 4096;

		block = malloc(blocksize);
		if (block == NULL)
		{
			result = ARSAL_ERROR_ALLOC;
		}
	}

	if (result == ARSAL_OK)
	{
		file = fopen(filePath, "rb");
		if (file == NULL)
		{
			result = ARSAL_ERROR_FILE;
		}
	}

	if (result == ARSAL_OK)
	{
		while ((count = fread(block, sizeof(uint8_t), blocksize, file)) > 0)
		{
			CryptHashData(hHash, block, count, 0);
		}
		CryptGetHashParam(hHash, HP_HASHVAL, md5, &md5Len, 0);
	}

	if (file != NULL)
	{
		fclose(file);
	}

	if (hHash != NULL)
	{
		CrpytDestroyHash(hHash);
		hHash = NULL;
	}

	if (block != NULL)
	{
		free(block);
		block = NULL;
	}
	
	return result;
}

static eARSAL_ERROR ARSAL_MD5_GetMd5AsTxt(const uint8_t *md5, int md5Len, char *md5Txt, int md5TxtLen)
{
	eARSAL_ERROR result = ARSAL_OK;
	int i;
	size_t digest_size = 32;

	if ((md5 == NULL) || (md5Len < digest_size) || (md5Txt == NULL) || (md5TxtLen < ((digest_size * 2) + 1)))
	{
		result = ARSAL_ERROR_BAD_PARAMETER;
	}

	for (i = 0; i < digest_size; i++)
	{
		sprintf(&md5Txt[i * 2], "%02x", md5[i]);
	}
	md5Txt[digest_size * 2] = '\0';

	return result;
}