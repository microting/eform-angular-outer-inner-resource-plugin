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
    // $('#newAreaBtn').waitForDisplayed(20000);
    $('#spinner-animation').waitForDisplayed(90000, true);
  });
  it('should add area with only name', function () {
    outerInnerResourceInnerResourcePage.newInnerResourceBtn.click();
    const newName = Guid.create().toString();
    $('#createInnerResourceName').waitForDisplayed(20000);
    outerInnerResourceModalPage.innerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
    $('#innerResourceDeleteBtn').waitForDisplayed(20000);
    listRowObject.deleteBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
  });
});
