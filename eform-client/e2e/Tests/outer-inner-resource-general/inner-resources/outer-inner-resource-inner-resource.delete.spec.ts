import outerInnerResourceInnerResourcePage , {ListRowObject} from '../../../Page objects/outer-inner-resource/outer-inner-resource-inner-resource.page';
import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';

const expect = require('chai').expect;

describe('Machine Area Area delete', function () {
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
    browser.waitForVisible('#createInnerResourceName', 20000);
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
    browser.pause(8000);
  });
  it('should delete area', function () {
    const listRowObject = new ListRowObject(1);
    listRowObject.deleteBtn.click();
    browser.waitForVisible('#innerResourceDeleteName', 20000);
    browser.pause(2000);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
    browser.pause(2000);
    browser.refresh();
    expect(listRowObject.id === null, 'Area is not deleted');

  });
});
