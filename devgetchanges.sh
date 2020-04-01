#!/bin/bash

cd ~

rm -fR Documents/workspace/microting/eform-angular-monitoring-plugin/eform-client/src/app/plugins/modules/outer-inner-resource-pn

cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/src/app/plugins/modules/outer-inner-resource-pn Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/src/app/plugins/modules/outer-inner-resource-pn

rm -fR Documents/workspace/microting/eform-angular-monitoring-plugin/eFormAPI/Plugins/OuterInnerResource.Pn

cp -a Documents/workspace/microting/eform-angular-frontend/eFormAPI/Plugins/OuterInnerResource.Pn Documents/workspace/microting/eform-angular-monitoring-plugin/eFormAPI/Plugins/OuterInnerResource.Pn

# Test files rm
rm -fR Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/monitoring-settings/
rm -fR Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/monitoring-general/
rm -fR Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Page\ objects/Monitoring/
rm -fR Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/wdio-headless-plugin-step2.conf.js

# Test files cp
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/monitoring-settings Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/monitoring-settings
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Tests/monitoring-general Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Tests/monitoring-general
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/e2e/Page\ objects/Monitoring Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/e2e/Page\ objects/Monitoring
cp -a Documents/workspace/microting/eform-angular-frontend/eform-client/wdio-plugin-step2.conf.js Documents/workspace/microting/eform-angular-outer-inner-resource-plugin/eform-client/wdio-headless-plugin-step2.conf.js
