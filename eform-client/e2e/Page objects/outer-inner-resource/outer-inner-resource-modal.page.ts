import Page from '../Page';

export class OuterInnerResourceModalPage extends Page {
    constructor() {
        super();
    }
    public get outerResourceCreateNameInput() {
        $('#createOuterResourceName').waitForDisplayed({timeout: 20000});
        $('#createOuterResourceName').waitForClickable({timeout: 20000});
        return $('#createOuterResourceName');
    }
    public get outerResourceCreateSaveBtn() {
        $('#outerResourceCreateSaveBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceCreateSaveBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceCreateSaveBtn');
    }
    public get outerResourceCreateCancelBtn() {
        $('#outerResourceCreateCancelBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceCreateCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceCreateCancelBtn');
    }
    public get outerResourceEditNameInput() {
        $('#updateOuterResourceName').waitForDisplayed({timeout: 20000});
        $('#updateOuterResourceName').waitForClickable({timeout: 20000});
        return $('#updateOuterResourceName');
    }
    public get outerResourceEditSaveBtn() {
        $('#outerResourceEditSaveBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceEditSaveBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceEditSaveBtn');
    }
    public get outerResourceEditCancelBtn() {
        $('#outerResourceEditCancelBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceEditCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceEditCancelBtn');
    }
    public get outerResourceDeleteAreaId() {
        $('#selectedOuterResourceId').waitForDisplayed({timeout: 20000});
        $('#selectedOuterResourceId').waitForClickable({timeout: 20000});
        return $('#selectedOuterResourceId');
    }
    public get outerResourceDeleteAreaName() {
        $('#selectedOuterResourceName').waitForDisplayed({timeout: 20000});
        $('#selectedOuterResourceName').waitForClickable({timeout: 20000});
        return $('#selectedOuterResourceName');
    }
    public get outerResourceDeleteDeleteBtn() {
        $('#outerResourceDeleteDeleteBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceDeleteDeleteBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceDeleteDeleteBtn');
    }
    public get outerResourceDeleteCancelBtn() {
        $('#outerResourceDeleteCancelBtn').waitForDisplayed({timeout: 20000});
        $('#outerResourceDeleteCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceDeleteCancelBtn');
    }
    public get innerResourceCreateNameInput() {
        $('#createInnerResourceName').waitForDisplayed({timeout: 20000});
        $('#createInnerResourceName').waitForClickable({timeout: 20000});
        return $('#createInnerResourceName');
    }
    public get innerResourceCreateSaveBtn() {
        $('#innerResourceCreateSaveBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceCreateSaveBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceCreateSaveBtn');
    }
    public get innerResourceCreateCancelBtn() {
        $('#innerResourceCreateCancelBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceCreateCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceCreateCancelBtn');
    }
    public get innerResourceEditName() {
        $('#updateInnerResourceName').waitForDisplayed({timeout: 20000});
        $('#updateInnerResourceName').waitForClickable({timeout: 20000});
        return $('#updateInnerResourceName');
    }
    public get innerResourceEditSaveBtn() {
        $('#innerResourceEditSaveBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceEditSaveBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceEditSaveBtn');
    }
    public get innerResourceEditCancelBtn() {
        $('#innerResourceEditCancelBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceEditCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceEditCancelBtn');
    }
    public get innerResourceDeleteName() {
        $('#innerResourceDeleteName').waitForDisplayed({timeout: 20000});
        $('#innerResourceDeleteName').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteName');
    }
    public get innerResourceDeleteDeleteBtn() {
        $('#innerResourceDeleteDeleteBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceDeleteDeleteBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteDeleteBtn');
    }
    public get innerResourceDeleteCancelBtn() {
        $('#innerResourceDeleteCancelBtn').waitForDisplayed({timeout: 20000});
        $('#innerResourceDeleteCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteCancelBtn');
    }
}

const outerInnerResourceModalPage = new OuterInnerResourceModalPage();
export default outerInnerResourceModalPage;
