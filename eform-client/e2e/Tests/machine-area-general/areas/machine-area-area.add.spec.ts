import machineAreaAreaPage, {ListRowObject} from '../../../Page objects/machine-area/mahcine-area-area.page';
import machineAreaModalPage from '../../../Page objects/machine-area/machine-area-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;

describe('Machine Area Area Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Machine Area area eForm';
    machineAreaAreaPage.createNewEform(newEformLabel);
    machineAreaAreaPage.goToAreas();
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should add area with only name', function () {
    machineAreaAreaPage.newAreaBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createAreaName', 20000);
    machineAreaModalPage.areaCreateNameInput.addValue(newName);
    machineAreaModalPage.areaCreateSaveBtn.click();
    browser.pause(8000);
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
    browser.refresh();
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(machineAreaAreaPage.rowNum());
    browser.waitForVisible('#areaDeleteBtn', 20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    machineAreaModalPage.areaDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
