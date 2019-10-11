import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';
import outerInnerResourceInnerResourcePage, {ListRowObject} from "../../../Page objects/outer-inner-resource/outer-inner-resource-inner-resource.page";

const expect = require('chai').expect;

describe('Machine Area Area Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    const newEformLabel = 'Machine Area area eForm';
    outerInnerResourceInnerResourcePage.createNewEform(newEformLabel);
    outerInnerResourceInnerResourcePage.goToInnerResource();
    // browser.waitForVisible('#newAreaBtn', 20000);
    browser.pause(8000);
  });
  it('should add area with only name', function () {
    outerInnerResourceInnerResourcePage.newInnerResourceBtn.click();
    const newName = Guid.create().toString();
    browser.waitForVisible('#createInnerResourceName', 20000);
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
    browser.pause(8000);
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
    browser.refresh();
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
    browser.waitForVisible('#innerResourceDeleteBtn', 20000);
    listRowObject.deleteBtn.click();
    browser.pause(2000);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
    browser.pause(5000);
    browser.refresh();
  });
});
