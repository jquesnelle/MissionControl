from os import listdir
from os.path import isfile, join, abspath,split
import subprocess

DUMPLIB_PATH = '\"J:\\Microsoft Visual Studio 14.0\\VC\\bin\\dumpbin.exe\" /LINKERMEMBER'

LIB_PATH = join(split(abspath(__file__))[0],join('..','Debug'))
LIB_FILES = [ f for f in listdir(LIB_PATH) if f.endswith('.lib') and isfile(join(LIB_PATH, f)) ]

FUNCTION_PREFIXES = ['_ARCONTROLLER']

out = open('ARDroneSDK3.def', 'w')
out.write('LIBRARY ARDroneSDK3\n')
out.write('EXPORTS\n')
ordinal = 1

for lib in LIB_FILES:
	cmd = DUMPLIB_PATH + ' \"' + join(LIB_PATH, lib) + '\"'
	print cmd
	p = subprocess.Popen(cmd, stdout=subprocess.PIPE)
	output = p.stdout.read()
	found_ps = False
	for line in output.splitlines():
		if found_ps == False:
			if line.find('public symbols') != -1:
				found_ps = True
		else:
			parts = line.split()
			if len(parts) == 2 and len([parts[1] for pre in FUNCTION_PREFIXES if parts[1].startswith(pre)]) != 0:
				fn_name = parts[1] if parts[1][0] != '_' else parts[1][1:]
				out.write(fn_name + ' @' + str(ordinal) + '\n')
				ordinal += 1

out.close()

print 'Wrote ARDroneSDK3.def with ' + str((ordinal - 1)) + ' functions\n'