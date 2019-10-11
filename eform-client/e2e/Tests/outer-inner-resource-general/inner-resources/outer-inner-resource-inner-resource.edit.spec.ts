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
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.newInnerResourceBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createAreaName');
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
    browser.pause(8000);
  });
  it('should edit area', function () {
    const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
    const newName = 'New Name';
    listRowObject.updateBtn.click();
    browser.waitForVisible('#updateAreaName');
    outerInnerResourceModalPage.innerResourceEditName.clearElement();
    outerInnerResourceModalPage.innerResourceEditName.addValue(newName);
    outerInnerResourceModalPage.innerResourceEditSaveBtn.click();
    browser.pause(2000);
    browser.refresh();
    browser.waitForVisible(listRowObject.updateBtn, 20000);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
    browser.waitForVisible('#areaDeleteBtn', 20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
