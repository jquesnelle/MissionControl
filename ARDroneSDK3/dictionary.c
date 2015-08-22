#include <stdlib.h>

#include <uthash/uthash.h>
#include <uthash/utlist.h>

#include <libARSAL/ARSAL_Print.h>

#include <libARController/ARCONTROLLER_Error.h>
#include <libARController/ARCONTROLLER_Dictionary.h>
#include <libARController/ARCONTROLLER_DICTIONARY_Key.h>


ARCONTROLLER_DICTIONARY_ELEMENT_t* GetDictionaryElement(ARCONTROLLER_DICTIONARY_ELEMENT_t *nativeDictionary, const char* key)
{
	ARCONTROLLER_DICTIONARY_ELEMENT_t *element = NULL;
	HASH_FIND_STR(nativeDictionary, key, element);
	return element;
}

ARCONTROLLER_DICTIONARY_ARG_t* GetDictionaryArg(ARCONTROLLER_DICTIONARY_ELEMENT_t *nativeDictionary, const char* key)
{
	ARCONTROLLER_DICTIONARY_ARG_t* arg = NULL;
	if (nativeDictionary != NULL)
		HASH_FIND_STR(nativeDictionary->arguments, key, arg);
	return arg;
}
