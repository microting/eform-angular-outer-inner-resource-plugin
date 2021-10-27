import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginPage from '../../Page objects/Plugin.page';

import { expect } from 'chai';

describe('Application settings page - site header section', function () {
  before(async () => {
    await loginPage.open('/auth');
    await loginPage.login();
  });
  it('should go to plugin settings page', async () => {
    await myEformsPage.Navbar.goToPluginsPage();
    await (await $('#plugin-name')).waitForDisplayed({ timeout: 50000 });

    const plugin = await pluginPage.getFirstPluginRowObj();
    expect(plugin.id).equal(1);
    expect(plugin.name).equal('Microting Outer Inner Resource plugin');
    expect(plugin.version).equal('1.0.0.0');
    expect(plugin.status, 'status is not equal').eq(false);
  });

  it('should activate the plugin', async () => {
    let plugin = await pluginPage.getFirstPluginRowObj();
    await plugin.enableOrDisablePlugin();

    plugin = await pluginPage.getFirstPluginRowObj();
    expect(plugin.id).equal(1);
    expect(plugin.name).equal('Microting Outer Inner Resource plugin');
    expect(plugin.version).equal('1.0.0.0');
    expect(plugin.status, 'status is not equal').eq(true);
  });
});
