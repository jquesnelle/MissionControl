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
 * @file libARSAL/ARSAL_Socket.c
 * @brief This file contains sources about socket abstraction layer
 * @date 06/06/2012
 * @author frederic.dhaeyer@parrot.com
 */
#include "config.h"
#include <stdlib.h>
#ifndef _WIN32
#include <unistd.h>
#endif
#include <libARSAL/ARSAL_Socket.h>
#include <errno.h>

int ARSAL_Socket_Create(int domain, int type, int protocol)
{
#ifdef _WIN32
	static int winsock_inited = 0;
	if (!winsock_inited)
	{
		WSADATA wsaData;
		int err = WSAStartup(MAKEWORD(2,2), &wsaData);
		if (err != 0 || LOBYTE(wsaData.wVersion) != 2 || HIBYTE(wsaData.wVersion) != 2)
			return -1;
		winsock_inited = 1;
	}
	return WSASocket(domain, type, protocol, NULL, 0, WSA_FLAG_OVERLAPPED);
#endif
    return socket(domain, type, protocol);
}

int ARSAL_Socket_Connect(int sockfd, const struct sockaddr *addr, socklen_t addrlen)
{
	int ret = connect(sockfd, addr, addrlen);
#ifdef _WIN32
	int wsaError;
	if (ret == SOCKET_ERROR)
	{
		wsaError = WSAGetLastError();
		if (wsaError == WSAEWOULDBLOCK)
			errno = EINPROGRESS;
	}
#endif
	return ret;
}

ssize_t ARSAL_Socket_Sendto(int sockfd, const void *buf, size_t buflen, int flags, const struct sockaddr *dest_addr, socklen_t addrlen)
{
    return sendto(sockfd, buf, buflen, flags, dest_addr, addrlen);
}

ssize_t ARSAL_Socket_Send(int sockfd, const void *buf, size_t buflen, int flags)
{
    ssize_t res;
    int tries = 10;
    int i;
    for (i = 0; i < tries; i++)
    {
        res = send(sockfd, buf, buflen, flags);
        if (res >= 0 || errno != ECONNREFUSED)
        {
            break;
        }
    }
    return res;
}

ssize_t ARSAL_Socket_Recvfrom(int sockfd, void *buf, size_t buflen, int flags, struct sockaddr *src_addr, socklen_t *addrlen)
{
    return recvfrom(sockfd, buf, buflen, flags, src_addr, addrlen);
}

ssize_t ARSAL_Socket_Recv(int sockfd, void *buf, size_t buflen, int flags)
{
    return recv(sockfd, buf, buflen, flags);
}

#ifndef _WIN32

ssize_t ARSAL_Socket_Writev (int sockfd, const struct iovec *iov, int iovcnt)
{
    return writev (sockfd, iov, iovcnt);
}

ssize_t ARSAL_Socket_Readv (int sockfd, const struct iovec *iov, int iovcnt)
{
    return readv (sockfd, iov, iovcnt);
}

#endif

int ARSAL_Socket_Bind(int sockfd, const struct sockaddr *addr, socklen_t addrlen)
{
    return bind(sockfd, addr, addrlen);
}

int ARSAL_Socket_Listen(int sockfd, int backlog)
{
    return listen(sockfd, backlog);
}

int ARSAL_Socket_Accept(int sockfd, struct sockaddr *addr, socklen_t *addrlen)
{
    return accept(sockfd, addr, addrlen);
}

int ARSAL_Socket_Close(int sockfd)
{
#ifndef _WIN32
    return close(sockfd);
#else
	return closesocket(sockfd);
#endif
}

int ARSAL_Socket_Setsockopt(int sockfd, int level, int optname, const void *optval, socklen_t optlen)
{
    return setsockopt(sockfd, level, optname, optval, optlen);
}
