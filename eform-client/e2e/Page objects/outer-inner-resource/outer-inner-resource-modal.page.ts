import Page from '../Page';

export class OuterInnerResourceModalPage extends Page {
  constructor() {
    super();
  }
  public get outerResourceCreateNameInput() {
    return browser.element('#createOuterResourceName');
  }
  public get outerResourceCreateSaveBtn() {
    return browser.element('#outerResourceCreateSaveBtn');
  }
  public get outerResourceCreateCancelBtn() {
    return browser.element('#outerResourceCreateCancelBtn');
  }
  public get outerResourceEditNameInput() {
    return browser.element('#updateOuterResourceName');
  }
  public get outerResourceEditSaveBtn() {
    return browser.element('#outerResourceEditSaveBtn');
  }
  public get outerResourceEditCancelBtn() {
    return browser.element('#outerResourceEditCancelBtn');
  }
  public get outerResourceDeleteAreaId() {
    return browser.element('#selectedOuterResourceId');
  }
  public get outerResourceDeleteAreaName() {
    return browser.element('#selectedOuterResourceName');
  }
  public get outerResourceDeleteDeleteBtn() {
    return browser.element('#outerResourceDeleteDeleteBtn');
  }
  public get outerResourceDeleteCancelBtn() {
    return browser.element('#outerResourceDeleteCancelBtn');
  }
  public get innerResourceCreateNameInput() {
    return browser.element('#createInnerResourceName');
  }
  public get innerResourceCreateSaveBtn() {
    return browser.element('#innerResourceCreateSaveBtn');
  }
  public get innerResourceCreateCancelBtn() {
    return browser.element('#innerResourceCreateCancelBtn');
  }
  public get innerResourceEditName() {
    return browser.element('#updateInnerResourceName');
  }
  public get innerResourceEditSaveBtn() {
    return browser.element('#innerResourceEditSaveBtn');
  }
  public get innerResourceEditCancelBtn() {
    return browser.element('#innerResourceEditCancelBtn');
  }
  public get innerResourceDeleteName() {
    return browser.element('#innerResourceDeleteName');
  }
  public get innerResourceDeleteDeleteBtn() {
    return browser.element('#innerResourceDeleteDeleteBtn');
  }
  public get innerResourceDeleteCancelBtn() {
    return browser.element('#innerResourceDeleteCancelBtn');
  }
}

const outerInnerResourceModalPage = new OuterInnerResourceModalPage();
export default outerInnerResourceModalPage;
