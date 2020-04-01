import outerInnerResourceOuterResourcePage , {ListRowObject} from '../../../Page objects/outer-inner-resource/outer-inner-resource-outer-resource.page';
import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;


describe('Machine Area Machine edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Machine Area machine eForm';
    outerInnerResourceOuterResourcePage.createNewEform(newEformLabel);
    outerInnerResourceOuterResourcePage.goToOuterResource();
    // $('#newAreaBtn').waitForDisplayed(20000);
    browser.pause(8000);
  });
  it('should create a new machine', function () {
    outerInnerResourceOuterResourcePage.newOuterResourceBtn.click();
    const newName = Guid.create().toString();
    $('#createOuterResourceName').waitForDisplayed(20000);
    outerInnerResourceModalPage.outerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.outerResourceCreateSaveBtn.click();
    browser.pause(8000);
  });
  // can't change name.
  // it('should edit machine', function () {
  //   const listRowObject = new ListRowObject(outerInnerResourceOuterResourcePage.rowNum());
  //   const newName = 'New Name';
  //   listRowObject.updateBtn.click();
  //   browser.waitForVisible('#updateMachineName');
  //   outerInnerResourceModalPage.outerResourceEditNameInput.clearElement();
  //   outerInnerResourceModalPage.outerResourceEditNameInput.addValue(newName);
  //   outerInnerResourceModalPage.outerResourceEditSaveBtn.click();
  //   browser.pause(2000);
  //   browser.refresh();
  //   browser.waitForVisible(listRowObject.updateBtn, 20000);
  //   expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  // });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceOuterResourcePage.rowNum());
    $('#outerResourceDeleteBtn').waitForDisplayed(20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    outerInnerResourceModalPage.outerResourceDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
