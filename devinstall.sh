#!/bin/bash
cd ~
pwd

if [ -d "Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/machnie-area-pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/machnie-area-pn
fi

cp -av Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/src/app/plugins/modules/machine-area-pn Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/machine-area-pn

if [ -d "Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/MachineArea.Pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/MachineArea.Pn
fi

cp -av Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eFormAPI/Plugins/MachineArea.Pn Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/MachineArea.Pn
