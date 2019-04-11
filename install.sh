#!/bin/bash

if [ ! -d "/var/www/microting/eform-angular-machinearea-plugin" ]; then
  cd /var/www/microting
  su ubuntu -c \
  "git clone https://github.com/microting/eform-angular-machinearea-plugin.git -b stable"
fi

cd /var/www/microting/eform-angular-machinearea-plugin
su ubuntu -c \
"dotnet restore eFormAPI/Plugins/MachineArea.Pn/MachineArea.Pn.sln"

echo "################## START GITVERSION ##################"
export GITVERSION=`git describe --abbrev=0 --tags | cut -d "v" -f 2`
echo $GITVERSION
echo "################## END GITVERSION ##################"
su ubuntu -c \
"dotnet publish eFormAPI/Plugins/MachineArea.Pn/MachineArea.Pn.sln -o out /p:Version=$GITVERSION --runtime linux-x64 --configuration Release"

su ubuntu -c \
"cp -av /var/www/microting/eform-angular-machinearea-plugin/eform-client/src/app/plugins/modules/machine-area-pn /var/www/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/machine-area-pn"
su ubuntu -c \
"mkdir -p /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/"
su ubuntu -c \
"cp -av /var/www/microting/eform-angular-machinearea-plugin/eFormAPI/Plugins/MachineArea.Pn/MachineArea.Pn/out /var/www/microting/eform-angular-frontend/eFormAPI/eFormAPI.Web/out/Plugins/MachineArea"


echo "Recompile angular"
cd /var/www/microting/eform-angular-frontend/eform-client
su ubuntu -c \
"/var/www/microting/eform-angular-machinearea-plugin/testinginstallpn.sh"
su ubuntu -c \
"npm run build"
echo "Recompiling angular done"


