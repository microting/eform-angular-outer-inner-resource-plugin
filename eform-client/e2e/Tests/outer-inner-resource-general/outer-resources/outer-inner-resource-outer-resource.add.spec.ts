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
    //outerInnerResourceOuterResourcePage.createNewEform(newEformLabel);
    outerInnerResourceOuterResourcePage.goToOuterResource();
    // $('#newAreaBtn').waitForDisplayed({timeout: 20000});
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should add machine with only name', function () {
    outerInnerResourceOuterResourcePage.newOuterResourceBtn.click();
    const newName = Guid.create().toString();
    $('#createOuterResourceName').waitForDisplayed({timeout: 20000});
    outerInnerResourceModalPage.outerResourceCreateNameInput.addValue(newName);
    outerInnerResourceModalPage.outerResourceCreateSaveBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const listRowObject = new ListRowObject(1);
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should clean up', function () {
    const listRowObject = new ListRowObject(outerInnerResourceOuterResourcePage.rowNum());
    $('#outerResourceDeleteBtn').waitForDisplayed({timeout: 20000});
    listRowObject.deleteBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    outerInnerResourceModalPage.outerResourceDeleteDeleteBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
});
