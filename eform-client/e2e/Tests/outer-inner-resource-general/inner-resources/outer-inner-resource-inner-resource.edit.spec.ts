import outerInnerResourceInnerResourcePage , {ListRowObject} from '../../../Page objects/outer-inner-resource/outer-inner-resource-inner-resource.page';
import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;

describe('Machine Area Area edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Number 1';
    // machineAreaAreaPage.createNewEform(newEformLabel);
    outerInnerResourceInnerResourcePage.goToInnerResource();
    // $('#newAreaBtn').waitForDisplayed(20000);
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.newInnerResourceBtn.click();
    const newName = Guid.create().toString();
    $('#createInnerResourceName').waitForDisplayed(20000);
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  });
  // TODO Can't change name.
  // it('should edit area', function () {
  //   const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
  //   const newName = 'New Name';
  //   listRowObject.updateBtn.click();
  //   $('#updateInnerResourceName').waitForDisplayed(20000);
  //   outerInnerResourceModalPage.innerResourceEditName.clearElement();
  //   outerInnerResourceModalPage.innerResourceEditName.addValue(newName);
  //   outerInnerResourceModalPage.innerResourceEditSaveBtn.click();
  //   $('#spinner-animation').waitForDisplayed(90000, true);
  //   browser.refresh();
  //   browser.waitForVisible(listRowObject.updateBtn, 20000);
  //   expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  // });
  it('should clean up', function () {
    browser.pause(500);
    const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
    $('#innerResourceDeleteBtn').waitForDisplayed(20000);
    listRowObject.deleteBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
  });
});
