import {PageWithNavbarPage} from '../PageWithNavbar.page';
import {Guid} from 'guid-typescript';
import XMLForEformFractions from '../../Constants/XMLForEformFractions';
import {parseTwoDigitYear} from 'moment';

export class OuterInnerResourceInnerResourcePage extends PageWithNavbarPage {
    constructor() {
        super();
    }

    public rowNum(): number {
        browser.pause(500);
        return $$('#tableBody > tr').length;
    }
    public get newEformBtn() {
        $('#newEFormBtn').waitForDisplayed(20000);
        $('#newEFormBtn').waitForClickable({timeout: 20000});
        return $('#newEFormBtn');
    }
    public get xmlTextArea() {
        $('#eFormXml').waitForDisplayed(20000);
        $('#eFormXml').waitForClickable({timeout: 20000});
        return $('#eFormXml');
    }
    public get createEformBtn() {
        $('#createEformBtn').waitForDisplayed(20000);
        $('#createEformBtn').waitForClickable({timeout: 20000});
        return $('#createEformBtn');
    }
    public get createEformTagSelector() {
        $('#createEFormMultiSelector').waitForDisplayed(20000);
        $('#createEFormMultiSelector').waitForClickable({timeout: 20000});
        return $('#createEFormMultiSelector');
    }
    public get createEformNewTagInput() {
        $('#addTagInput').waitForDisplayed(20000);
        $('#addTagInput').waitForClickable({timeout: 20000});
        return $('#addTagInput');
    }
    public get outerInnerResourceDropdownMenu() {
        $('#outer-inner-resource-pn').waitForDisplayed(20000);
        $('#outer-inner-resource-pn').waitForClickable({timeout: 20000});
        return $('#outer-inner-resource-pn');
    }
    public get innerResourceMenuPoint() {
        $('#outer-inner-resource-pn-inner-resources').waitForDisplayed(20000);
        $('#outer-inner-resource-pn-inner-resources').waitForClickable({timeout: 20000});
        return $('#outer-inner-resource-pn-inner-resources');
    }
    public get newInnerResourceBtn() {
        $('#newInnerResource').waitForDisplayed(20000);
        $('#newInnerResource').waitForClickable({timeout: 20000});
        return $('#newInnerResource');
    }

    goToInnerResource() {
        this.outerInnerResourceDropdownMenu.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
        this.innerResourceMenuPoint.click();
        $('#newInnerResource').waitForDisplayed(20000);
    }

    createNewEform(eFormLabel, newTagsList = [], tagAddedNum = 0) {
        this.newEformBtn.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
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
            $('#spinner-animation').waitForDisplayed(90000, true);
        }
        // Add existing tags
        const selectedTags: string[] = [];
        if (tagAddedNum > 0) {
            $('#spinner-animation').waitForDisplayed(90000, true);
            for (let i = 0; i < tagAddedNum; i++) {
                this.createEformTagSelector.click();
                const selectedTag = $('.ng-option:not(.ng-option-selected)');
                selectedTags.push(selectedTag.getText());
                console.log('selectedTags is ' + JSON.stringify(selectedTags));
                selectedTag.click();
                $('#spinner-animation').waitForDisplayed(90000, true);
            }
        }
        this.createEformBtn.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
        return {added: addedTags, selected: selectedTags};
    }
}
const outerInnerResourceInnerResourcePage = new OuterInnerResourceInnerResourcePage();
export default outerInnerResourceInnerResourcePage;

export class ListRowObject {
    constructor(rowNum) {
        if ($$('#innerResourceId')[rowNum - 1]) {
            try {
                this.name = $$('#innerResourceName')[rowNum - 1].getText();
            } catch (e) {}
            try {
                this.updateBtn = $$('#innerResourceEditBtn')[rowNum - 1];
            } catch (e) {}
            try {
                this.deleteBtn = $$('#innerResourceDeleteBtn')[rowNum - 1];
            } catch (e) {}
        }
    }
    public id;
    public name;
    public updateBtn;
    public deleteBtn;
}
