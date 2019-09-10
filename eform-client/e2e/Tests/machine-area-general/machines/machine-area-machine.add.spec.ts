import machineAreaMachinePage, {ListRowObject} from '../../../Page objects/machine-area/machine-area-machine.page';
import machineAreaModalPage from '../../../Page objects/machine-area/machine-area-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;


describe('Machine Area Machine Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Machine Area machine eForm';
    machineAreaMachinePage.createNewEform(newEformLabel);
    machineAreaMachinePage.goToMachines();
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should add machine with only name', function () {
    machineAreaMachinePage.newMachineBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createMachineName', 20000);
    machineAreaModalPage.machineCreateNameInput.addValue(newName);
    machineAreaModalPage.machineCreateSaveBtn.click();
    browser.pause(8000);
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
    browser.refresh();
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(machineAreaMachinePage.rowNum());
    browser.waitForVisible('#machineDeleteBtn', 20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    machineAreaModalPage.machineDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
