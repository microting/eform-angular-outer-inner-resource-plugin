import machineAreaAreaPage, {ListRowObject} from '../../../Page objects/machine-area/mahcine-area-area.page';
import machineAreaModalPage from '../../../Page objects/machine-area/machine-area-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;

describe('Machine Area Area edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Number 1';
    // machineAreaAreaPage.createNewEform(newEformLabel);
    machineAreaAreaPage.goToAreas();
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should create a new area', function () {
    machineAreaAreaPage.newAreaBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createAreaName');
    machineAreaModalPage.areaCreateNameInput.addValue(newName);
    machineAreaModalPage.areaCreateSaveBtn.click();
    browser.pause(8000);
  });
  it('should edit area', function () {
    const listRowObject = new ListRowObject(machineAreaAreaPage.rowNum());
    const newName = 'New Name';
    listRowObject.updateBtn.click();
    browser.waitForVisible('#updateAreaName');
    machineAreaModalPage.areaEditNameInput.clearElement();
    machineAreaModalPage.areaEditNameInput.addValue(newName);
    machineAreaModalPage.areaEditSaveBtn.click();
    browser.pause(2000);
    browser.refresh();
    browser.waitForVisible(listRowObject.updateBtn, 20000);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
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
