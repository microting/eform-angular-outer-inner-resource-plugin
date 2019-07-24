import {PageWithNavbarPage} from '../PageWithNavbar.page';
import {Guid} from 'guid-typescript';
import XMLForEformFractions from '../../Constants/XMLForEformFractions';
import {parseTwoDigitYear} from 'moment';

export class MachineAreaMachinePage extends PageWithNavbarPage {
  constructor() {
    super();
  }

  public rowNum(): number {
    return browser.$$('#tableBody > tr').length;
  }
  public get newEformBtn() {
    return browser.element('#newEFormBtn');
  }
  public get xmlTextArea() {
    return browser.element('#eFormXml');
  }
  public get createEformBtn() {
    return browser.element('#createEformBtn');
  }
  public get createEformTagSelector() {
    return browser.element('#createEFormMultiSelector');
  }
  public get createEformNewTagInput() {
    return browser.element('#addTagInput');
  }
  public get machineAreaDropdownMenu() {
    return browser.element('#machine-area-pn');
  }
  public get machineMenuPoint() {
    return browser.element('#machine-area-pn-Machines');
  }
  public get newMachineBtn() {
    return browser.element('#newMachineBtn');
  }

  goToMachines() {
    this.machineAreaDropdownMenu.click();
    browser.pause(2000);
    this.machineMenuPoint.click();
  }

  createNewEform(eFormLabel, newTagsList = [], tagAddedNum = 0) {
    this.newEformBtn.click();
    browser.pause(5000);
    // Create replaced xml and insert it in textarea
    const xml = XMLForEformFractions.XML.replace('TEST_LABEL', eFormLabel);
    browser.execute(function (xmlText) {
      (<HTMLInputElement>document.getElementById('eFormXml')).value = xmlText;
    }, xml);
    this.xmlTextArea.addValue(' ');
    // Create new tags
    const addedTags: string[] = newTagsList;
    if (newTagsList.length > 0) {
      this.createEformNewTagInput.setValue(newTagsList.join(','));
      browser.pause(5000);
    }
    // Add existing tags
    const selectedTags: string[] = [];
    if (tagAddedNum > 0) {
      browser.pause(5000);
      for (let i = 0; i < tagAddedNum; i++) {
        this.createEformTagSelector.click();
        const selectedTag = $('.ng-option:not(.ng-option-selected)');
        selectedTags.push(selectedTag.getText());
        console.log('selectedTags is ' + JSON.stringify(selectedTags));
        selectedTag.click();
        browser.pause(5000);
      }
    }
    this.createEformBtn.click();
    browser.pause(14000);
    return {added: addedTags, selected: selectedTags};
  }
}
const machineAreaMachinePage = new MachineAreaMachinePage();
export default machineAreaMachinePage;

export class ListRowObject {
  constructor(rowNum) {
    if ($$('#machineId')[rowNum - 1]) {
      try {
        this.name = $$('#machineName')[rowNum - 1].getText();
      } catch (e) {}
      try {
        this.updateBtn = $$('#machineEditBtn')[rowNum - 1];
      } catch (e) {}
      try {
        this.deleteBtn = $$('#machineDeleteBtn')[rowNum - 1];
      } catch (e) {}
    }
  }
  public id;
  public name;
  public updateBtn;
  public deleteBtn;
}
