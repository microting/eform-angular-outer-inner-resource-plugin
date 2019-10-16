#!/bin/bash
cd ~
pwd

if [ -d "Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn
fi

cp -av Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/src/app/plugins/modules/outer-inner-resource-pn Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn

if [ -d "Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn" ]; then
	rm -fR Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn
fi

cp -av Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eFormAPI/Plugins/OuterInnerResource.Pn Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn
