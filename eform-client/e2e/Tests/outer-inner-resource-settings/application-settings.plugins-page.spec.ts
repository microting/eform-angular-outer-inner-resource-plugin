import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginPage from '../../Page objects/Plugin.page';

import {expect} from 'chai';
import pluginsPage from './application-settings.plugins.page';

describe('Application settings page - site header section', function () {
    before(function () {
        loginPage.open('/auth');
    });
    it('should go to plugin settings page', function () {
        loginPage.login();
        myEformsPage.Navbar.advancedDropdownClick();
        myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
        $('#plugin-name').waitForDisplayed({timeout: 50000});
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});

        const plugin = pluginsPage.getFirstPluginRowObj();
        expect(plugin.id).equal(1);
        expect(plugin.name).equal('Microting Outer Inner Resource plugin');
        expect(plugin.version).equal('1.0.0.0');

    });

    it('should activate the plugin', function () {
        pluginPage.pluginSettingsBtn.click();
        $('#pluginOKBtn').waitForDisplayed({timeout: 40000});
        pluginPage.pluginOKBtn.click();
        browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
        loginPage.open('/');

        loginPage.login();
        myEformsPage.Navbar.advancedDropdownClick();
        myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
        $('#plugin-name').waitForDisplayed({timeout: 50000});
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});

        const plugin = pluginsPage.getFirstPluginRowObj();
        expect(plugin.id).equal(1);
        expect(plugin.name).equal('Microting Outer Inner Resource plugin');
        expect(plugin.version).equal('1.0.0.0');
        plugin.settingsBtn.waitForDisplayed({timeout: 20000});
    });
});
