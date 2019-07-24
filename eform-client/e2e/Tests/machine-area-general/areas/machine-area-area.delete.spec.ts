import machineAreaAreaPage, {ListRowObject} from '../../../Page objects/machine-area/mahcine-area-area.page';
import machineAreaModalPage from '../../../Page objects/machine-area/machine-area-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;

describe('Machine Area Area delete', function () {
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
  it('should delete area', function () {
    const listRowObject = new ListRowObject(machineAreaAreaPage.rowNum());
    listRowObject.deleteBtn.click();
    browser.waitForVisible('#selectedAreaId');
    browser.pause(2000);
    machineAreaModalPage.areaDeleteDeleteBtn.click();
    browser.pause(2000);
    browser.refresh();
    expect(listRowObject.id === null, 'Area is not deleted');

  });
});
