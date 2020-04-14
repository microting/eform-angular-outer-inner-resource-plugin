import {PageWithNavbarPage} from '../PageWithNavbar.page';
import {Guid} from 'guid-typescript';
import XMLForEformFractions from '../../Constants/XMLForEformFractions';
import {parseTwoDigitYear} from 'moment';

export class OuterInnerResourceOuterResourcePage extends PageWithNavbarPage {
    constructor() {
        super();
    }

    public rowNum(): number {
        browser.pause(500);
        return $$('#tableBody > tr').length;
    }
    public get newEformBtn() {
        $('#newEFormBtn').waitForDisplayed({timeout: 20000});
        $('#newEFormBtn').waitForClickable({timeout: 20000});
        return $('#newEFormBtn');
    }
    public get xmlTextArea() {
        $('#eFormXml').waitForDisplayed({timeout: 20000});
        $('#eFormXml').waitForClickable({timeout: 20000});
        return $('#eFormXml');
    }
    public get createEformBtn() {
        $('#createEformBtn').waitForDisplayed({timeout: 20000});
        $('#createEformBtn').waitForClickable({timeout: 20000});
        return $('#createEformBtn');
    }
    public get createEformTagSelector() {
        $('#createEFormMultiSelector').waitForDisplayed({timeout: 20000});
        $('#createEFormMultiSelector').waitForClickable({timeout: 20000});
        return $('#createEFormMultiSelector');
    }
    public get createEformNewTagInput() {
        $('#addTagInput').waitForDisplayed({timeout: 20000});
        $('#addTagInput').waitForClickable({timeout: 20000});
        return $('#addTagInput');
    }
    public get outerResourceName() {
        $('#areaName').waitForDisplayed({timeout: 20000});
        $('#areaName').waitForClickable({timeout: 20000});
        return $('#areaName');
    }
    public get outerResourceId() {
        $('#areaId').waitForDisplayed({timeout: 20000});
        $('#areaId').waitForClickable({timeout: 20000});
        return $('#areaId');
    }
    public get outerInnerResourceDropdownMenu() {
        $('#outer-inner-resource-pn').waitForDisplayed({timeout: 20000});
        $('#outer-inner-resource-pn').waitForClickable({timeout: 20000});
        return $('#outer-inner-resource-pn');
    }
    public get outerResourceMenuPoint() {
        $('#outer-inner-resource-pn-outer-resources').waitForDisplayed({timeout: 20000});
        $('#outer-inner-resource-pn-outer-resources').waitForClickable({timeout: 20000});
        return $('#outer-inner-resource-pn-outer-resources');
    }
    public get newOuterResourceBtn() {
        $('#newOuterResourceBtn').waitForDisplayed({timeout: 20000});
        $('#newOuterResourceBtn').waitForClickable({timeout: 20000});
        return $('#newOuterResourceBtn');
    }

    goToOuterResource() {
        this.outerInnerResourceDropdownMenu.click();
        $('#outer-inner-resource-pn-outer-resources').waitForDisplayed({timeout: 20000});
        this.outerResourceMenuPoint.click();
        $('#newOuterResourceBtn').waitForDisplayed({timeout: 20000});
    }

    createNewEform(eFormLabel, newTagsList = [], tagAddedNum = 0) {
        this.newEformBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
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
            $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
        }
        // Add existing tags
        const selectedTags: string[] = [];
        if (tagAddedNum > 0) {
            $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
            for (let i = 0; i < tagAddedNum; i++) {
                this.createEformTagSelector.click();
                const selectedTag = $('.ng-option:not(.ng-option-selected)');
                selectedTags.push(selectedTag.getText());
                console.log('selectedTags is ' + JSON.stringify(selectedTags));
                selectedTag.click();
                $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
            }
        }
        this.createEformBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
        return {added: addedTags, selected: selectedTags};
    }
}

const outerInnerResourceOuterResourcePage = new OuterInnerResourceOuterResourcePage();
export default outerInnerResourceOuterResourcePage;

export class ListRowObject {
    constructor(rowNum) {
        if ($$('#outerResourceId')[rowNum - 1]) {
            try {
                this.name = $$('#outerResourceName')[rowNum - 1].getText();
            } catch (e) {}
            try {
                this.updateBtn = $$('#outerResourceEditBtn')[rowNum - 1];
            } catch (e) {}
            try {
                this.deleteBtn = $$('#outerResourceDeleteBtn')[rowNum - 1];
            } catch (e) {}
        }
    }
    public id;
    public name;
    public updateBtn;
    public deleteBtn;
}
