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
    // $('#newAreaBtn').waitForDisplayed(20000);
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.newInnerResourceBtn.click();
    const newName = Guid.create().toString();
    $('#createInnerResourceName').waitForDisplayed(20000);
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
  });
  it('should delete area', function () {
    const listRowObject = new ListRowObject(1);
    listRowObject.deleteBtn.click();
    $('#innerResourceDeleteName').waitForDisplayed(20000);
    $('#spinner-animation').waitForDisplayed(90000, true);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    expect(listRowObject.id === null, 'Area is not deleted');

  });
});
