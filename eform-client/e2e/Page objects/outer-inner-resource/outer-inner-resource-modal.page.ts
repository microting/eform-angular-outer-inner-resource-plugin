import Page from '../Page';

export class OuterInnerResourceModalPage extends Page {
    constructor() {
        super();
    }
    public get outerResourceCreateNameInput() {
        $('#createOuterResourceName').waitForDisplayed(20000);
        $('#createOuterResourceName').waitForClickable({timeout: 20000});
        return $('#createOuterResourceName');
    }
    public get outerResourceCreateSaveBtn() {
        $('#outerResourceCreateSaveBtn').waitForDisplayed(20000);
        $('#outerResourceCreateSaveBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceCreateSaveBtn');
    }
    public get outerResourceCreateCancelBtn() {
        $('#outerResourceCreateCancelBtn').waitForDisplayed(20000);
        $('#outerResourceCreateCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceCreateCancelBtn');
    }
    public get outerResourceEditNameInput() {
        $('#updateOuterResourceName').waitForDisplayed(20000);
        $('#updateOuterResourceName').waitForClickable({timeout: 20000});
        return $('#updateOuterResourceName');
    }
    public get outerResourceEditSaveBtn() {
        $('#outerResourceEditSaveBtn').waitForDisplayed(20000);
        $('#outerResourceEditSaveBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceEditSaveBtn');
    }
    public get outerResourceEditCancelBtn() {
        $('#outerResourceEditCancelBtn').waitForDisplayed(20000);
        $('#outerResourceEditCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceEditCancelBtn');
    }
    public get outerResourceDeleteAreaId() {
        $('#selectedOuterResourceId').waitForDisplayed(20000);
        $('#selectedOuterResourceId').waitForClickable({timeout: 20000});
        return $('#selectedOuterResourceId');
    }
    public get outerResourceDeleteAreaName() {
        $('#selectedOuterResourceName').waitForDisplayed(20000);
        $('#selectedOuterResourceName').waitForClickable({timeout: 20000});
        return $('#selectedOuterResourceName');
    }
    public get outerResourceDeleteDeleteBtn() {
        $('#outerResourceDeleteDeleteBtn').waitForDisplayed(20000);
        $('#outerResourceDeleteDeleteBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceDeleteDeleteBtn');
    }
    public get outerResourceDeleteCancelBtn() {
        $('#outerResourceDeleteCancelBtn').waitForDisplayed(20000);
        $('#outerResourceDeleteCancelBtn').waitForClickable({timeout: 20000});
        return $('#outerResourceDeleteCancelBtn');
    }
    public get innerResourceCreateNameInput() {
        $('#createInnerResourceName').waitForDisplayed(20000);
        $('#createInnerResourceName').waitForClickable({timeout: 20000});
        return $('#createInnerResourceName');
    }
    public get innerResourceCreateSaveBtn() {
        $('#innerResourceCreateSaveBtn').waitForDisplayed(20000);
        $('#innerResourceCreateSaveBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceCreateSaveBtn');
    }
    public get innerResourceCreateCancelBtn() {
        $('#innerResourceCreateCancelBtn').waitForDisplayed(20000);
        $('#innerResourceCreateCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceCreateCancelBtn');
    }
    public get innerResourceEditName() {
        $('#updateInnerResourceName').waitForDisplayed(20000);
        $('#updateInnerResourceName').waitForClickable({timeout: 20000});
        return $('#updateInnerResourceName');
    }
    public get innerResourceEditSaveBtn() {
        $('#innerResourceEditSaveBtn').waitForDisplayed(20000);
        $('#innerResourceEditSaveBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceEditSaveBtn');
    }
    public get innerResourceEditCancelBtn() {
        $('#innerResourceEditCancelBtn').waitForDisplayed(20000);
        $('#innerResourceEditCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceEditCancelBtn');
    }
    public get innerResourceDeleteName() {
        $('#innerResourceDeleteName').waitForDisplayed(20000);
        $('#innerResourceDeleteName').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteName');
    }
    public get innerResourceDeleteDeleteBtn() {
        $('#innerResourceDeleteDeleteBtn').waitForDisplayed(20000);
        $('#innerResourceDeleteDeleteBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteDeleteBtn');
    }
    public get innerResourceDeleteCancelBtn() {
        $('#innerResourceDeleteCancelBtn').waitForDisplayed(20000);
        $('#innerResourceDeleteCancelBtn').waitForClickable({timeout: 20000});
        return $('#innerResourceDeleteCancelBtn');
    }
}

const outerInnerResourceModalPage = new OuterInnerResourceModalPage();
export default outerInnerResourceModalPage;
