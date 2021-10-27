#!/bin/bash

if [ ! -d "/var/www/microting/eform-angular-outer-inner-resource-plugin" ]; then
	rm -fR /var/www/microting/eform-angular-outer-inner-resource-plugin
  cd /var/www/microting
  su ubuntu -c \
  "git clone https://github.com/microting/eform-angular-outer-inner-resource-plugin.git -b stable"
fi

cd /var/www/microting/eform-angular-outer-inner-resource-plugin
su ubuntu -c \
"dotnet restore eFormAPI/Plugins/OuterInnerResource.Pn/OuterInnerResource.Pn.sln"

echo "################## START GITVERSION ##################"
export GITVERSION=`git describe --abbrev=0 --tags | cut -d "v" -f 2`
echo $GITVERSION
echo "################## END GITVERSION ##################"
su ubuntu -c \
"dotnet publish eFormAPI/Plugins/OuterInnerResource.Pn/OuterInnerResource.Pn.sln -o out /p:Version=$GITVERSION --runtime linux-x64 --configuration Release"

if [ -d "/var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn" ]; then
	su ubuntu -c \
	"rm -fR /var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn"
fi

su ubuntu -c \
"cp -av /var/www/microting/eform-angular-outer-inner-resource-plugin/eform-client/src/app/plugins/modules/outer-inner-resource-pn /var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn"
su ubuntu -c \
"mkdir -p /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/"

if [ -d "/var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/OuterInnerResource" ]; then
	su ubuntu -c \
	"rm -fR /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/OuterInnerResource"
fi

su ubuntu -c \
"cp -av /var/www/microting/eform-angular-outer-inner-resource-plugin/eFormAPI/Plugins/OuterInnerResource.Pn/OuterInnerResource.Pn/out /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/OuterInnerResource"


echo "Recompile angular"
cd /var/www/microting/eform-angular-frontend/eform-client
su ubuntu -c \
"/var/www/microting/eform-angular-outer-inner-resource-plugin/testinginstallpn.sh"
su ubuntu -c \
"export NODE_OPTIONS=--max_old_space_size=8192 && time GENERATE_SOURCEMAP=false npm run build"
echo "Recompiling angular done"
/rabbitmqadmin declare queue name=eform-angular-outer-inner-resource-plugin durable=true
