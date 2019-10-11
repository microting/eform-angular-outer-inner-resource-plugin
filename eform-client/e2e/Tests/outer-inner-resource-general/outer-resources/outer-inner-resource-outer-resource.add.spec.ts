import outerInnerResourceOuterResourcePage , {ListRowObject} from '../../../Page objects/outer-inner-resource/outer-inner-resource-outer-resource.page';
import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;


describe('Machine Area Machine Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Machine Area machine eForm';
    outerInnerResourceOuterResourcePage.createNewEform(newEformLabel);
    outerInnerResourceOuterResourcePage.goToOuterResource();
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should add machine with only name', function () {
    outerInnerResourceOuterResourcePage.newOuterResourceBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createMachineName', 20000);
    outerInnerResourceModalPage.outerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.outerResourceCreateSaveBtn.click();
    browser.pause(8000);
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
    browser.refresh();
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceOuterResourcePage.rowNum());
    browser.waitForVisible('#machineDeleteBtn', 20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    outerInnerResourceModalPage.outerResourceDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
