import { Page, Locator } from '@playwright/test';
import { OuterInnerResourceModalPage } from './OuterInnerResourceModal.page';

export class OuterInnerResourceInnerResourcePage {
  public modalPage: OuterInnerResourceModalPage;

  constructor(public page: Page) {
    this.modalPage = new OuterInnerResourceModalPage(page);
  }

  public async rowNum(): Promise<number> {
    await this.page.waitForTimeout(500);
    return await this.page.locator('tbody > tr').count();
  }

  public outerInnerResourceDropdownMenu(): Locator {
    return this.page.locator('#outer-inner-resource-pn');
  }

  public innerResourceMenuPoint(): Locator {
    return this.page.locator('#outer-inner-resource-pn-inner-resources');
  }

  public newInnerResourceBtn(): Locator {
    return this.page.locator('#newInnerResource');
  }

  async goToInnerResource() {
    await this.outerInnerResourceDropdownMenu().waitFor({ state: 'visible', timeout: 20000 });
    await this.outerInnerResourceDropdownMenu().click();
    await this.innerResourceMenuPoint().waitFor({ state: 'visible', timeout: 20000 });
    await this.innerResourceMenuPoint().click();
    await this.newInnerResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
  }

  async getInnerObjectByName(name: string): Promise<InnerResourceListRowObject | null> {
    await this.page.waitForTimeout(500);
    const count = await this.rowNum();
    for (let i = 1; i <= count; i++) {
      const listRowObject = await this.getRowObject(i);
      if (listRowObject.name === name) {
        return listRowObject;
      }
    }
    return null;
  }

  async getRowObject(rowNum: number): Promise<InnerResourceListRowObject> {
    const obj = new InnerResourceListRowObject(this);
    return await obj.getRow(rowNum);
  }

  async openCreateModal(name?: string, externalId?: number | string) {
    await this.newInnerResourceBtn().click();
    if (name) {
      await this.modalPage.innerResourceCreateNameInput().waitFor({ state: 'visible', timeout: 20000 });
      await this.modalPage.innerResourceCreateNameInput().fill(name);
    }
    if (externalId) {
      await this.modalPage.createInnerResourceId().waitFor({ state: 'visible', timeout: 20000 });
      await this.modalPage.createInnerResourceId().fill(externalId.toString());
    }
  }

  async closeCreateModal(clickCancel = false) {
    if (!clickCancel) {
      await this.modalPage.innerResourceCreateSaveBtn().click();
      await this.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.modalPage.innerResourceCreateCancelBtn().click();
      await this.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
      await this.newInnerResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async createNewInnerResource(name?: string, externalId?: number | string, clickCancel = false) {
    await this.openCreateModal(name, externalId);
    await this.closeCreateModal(clickCancel);
  }
}

export class InnerResourceListRowObject {
  public id: number;
  public name: string;
  public externalId: number;
  public rowLocator: Locator;
  private pageObj: OuterInnerResourceInnerResourcePage;

  constructor(pageObj: OuterInnerResourceInnerResourcePage) {
    this.pageObj = pageObj;
  }

  async getRow(rowNum: number): Promise<InnerResourceListRowObject> {
    this.rowLocator = this.pageObj.page.locator('tbody > tr').nth(rowNum - 1);
    if (await this.rowLocator.count() > 0) {
      try {
        this.id = +(await this.rowLocator.locator('.mat-column-id').innerText());
      } catch (e) {}
      try {
        this.name = await this.rowLocator.locator('.mat-column-name').innerText();
      } catch (e) {}
      try {
        this.externalId = +(await this.rowLocator.locator('.mat-column-externalId').innerText());
      } catch (e) {}
    }
    return this;
  }

  async openDeleteModal() {
    await this.rowLocator.locator('.mat-column-actions button').nth(1).click();
    await this.pageObj.modalPage.innerResourceDeleteDeleteBtn().waitFor({ state: 'visible', timeout: 20000 });
  }

  async closeDeleteModal(clickCancel = false) {
    if (!clickCancel) {
      await this.pageObj.modalPage.innerResourceDeleteDeleteBtn().click();
      await this.pageObj.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.pageObj.modalPage.innerResourceDeleteCancelBtn().click();
      await this.pageObj.newInnerResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async delete(clickCancel = false) {
    await this.openDeleteModal();
    await this.closeDeleteModal(clickCancel);
  }

  async openEditModal(newName?: string, newExternalId?: number | string) {
    await this.rowLocator.locator('.mat-column-actions button').nth(0).click();
    if (newName) {
      await this.pageObj.modalPage.innerResourceEditName().waitFor({ state: 'visible', timeout: 20000 });
      await this.pageObj.modalPage.innerResourceEditName().fill(newName);
    }
    if (newExternalId) {
      await this.pageObj.modalPage.innerResourceEditExternalIdInput().waitFor({ state: 'visible', timeout: 20000 });
      await this.pageObj.modalPage.innerResourceEditExternalIdInput().fill(newExternalId.toString());
    }
  }

  async closeEditModal(clickCancel = false) {
    if (!clickCancel) {
      await this.pageObj.modalPage.innerResourceEditSaveBtn().click();
      await this.pageObj.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.pageObj.modalPage.innerResourceEditCancelBtn().click();
      await this.pageObj.newInnerResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async edit(newName?: string, newExternalId?: number | string, clickCancel = false) {
    await this.openEditModal(newName, newExternalId);
    await this.closeEditModal(clickCancel);
  }
}
