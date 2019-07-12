import Page from '../Page';

export class MachineAreaModalPage extends Page {
  constructor() {
    super();
  }
  public get areaCreateNameInput() {
    return browser.element('#createAreaName');
  }
  public get areaCreateSaveBtn() {
    return browser.element('#areaCreateSaveBtn');
  }
  public get areaCreateCancelBtn() {
    return browser.element('#areaCreateCancelBtn');
  }
  public get areaEditNameInput() {
    return browser.element('#updateAreaName');
  }
  public get areaEditSaveBtn() {
    return browser.element('#areaEditSaveBtn');
  }
  public get areaEditCancelBtn() {
    return browser.element('#areaEditCancelBtn');
  }
  public get areaDeleteAreaId() {
    return browser.element('#selectedAreaId');
  }
  public get areaDeleteAreaName() {
    return browser.element('#selectedAreaName');
  }
  public get areaDeleteDeleteBtn() {
    return browser.element('#areaDeleteDeleteBtn');
  }
  public get areaDeleteCancelBtn() {
    return browser.element('#areaDeleteCancelBtn');
  }
}
