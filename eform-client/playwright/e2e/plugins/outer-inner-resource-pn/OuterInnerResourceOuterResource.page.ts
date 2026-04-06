import { Page, Locator } from '@playwright/test';
import { OuterInnerResourceModalPage } from './OuterInnerResourceModal.page';

export class OuterInnerResourceOuterResourcePage {
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

  public outerResourceMenuPoint(): Locator {
    return this.page.locator('#outer-inner-resource-pn-outer-resources');
  }

  public newOuterResourceBtn(): Locator {
    return this.page.locator('#newOuterResourceBtn');
  }

  async goToOuterResource() {
    await this.outerInnerResourceDropdownMenu().waitFor({ state: 'visible', timeout: 20000 });
    await this.outerInnerResourceDropdownMenu().click();
    await this.outerResourceMenuPoint().waitFor({ state: 'visible', timeout: 20000 });
    await this.outerResourceMenuPoint().click();
    await this.newOuterResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
  }

  async getOuterObjectByName(name: string): Promise<OuterResourceListRowObject | null> {
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

  async getRowObject(rowNum: number): Promise<OuterResourceListRowObject> {
    const obj = new OuterResourceListRowObject(this);
    return await obj.getRow(rowNum);
  }

  async openCreateModal(name?: string, externalId?: number | string) {
    await this.newOuterResourceBtn().click();
    if (name) {
      await this.modalPage.outerResourceCreateNameInput().waitFor({ state: 'visible', timeout: 20000 });
      await this.modalPage.outerResourceCreateNameInput().fill(name);
    }
    if (externalId) {
      await this.modalPage.createOuterResourceExternalId().waitFor({ state: 'visible', timeout: 20000 });
      await this.modalPage.createOuterResourceExternalId().fill(externalId.toString());
    }
  }

  async closeCreateModal(clickCancel = false) {
    if (!clickCancel) {
      await this.modalPage.outerResourceCreateSaveBtn().click();
      await this.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.modalPage.outerResourceCreateCancelBtn().click();
      await this.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
      await this.newOuterResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async createNewOuterResource(name?: string, externalId?: number | string, clickCancel = false) {
    await this.openCreateModal(name, externalId);
    await this.closeCreateModal(clickCancel);
  }
}

export class OuterResourceListRowObject {
  public id: number;
  public name: string;
  public externalId: number;
  public rowLocator: Locator;
  private pageObj: OuterInnerResourceOuterResourcePage;

  constructor(pageObj: OuterInnerResourceOuterResourcePage) {
    this.pageObj = pageObj;
  }

  async getRow(rowNum: number): Promise<OuterResourceListRowObject> {
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
    await this.pageObj.modalPage.outerResourceDeleteDeleteBtn().waitFor({ state: 'visible', timeout: 20000 });
  }

  async closeDeleteModal(clickCancel = false) {
    if (!clickCancel) {
      await this.pageObj.modalPage.outerResourceDeleteDeleteBtn().click();
      await this.pageObj.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.pageObj.modalPage.outerResourceDeleteCancelBtn().click();
      await this.pageObj.newOuterResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async delete(clickCancel = false) {
    await this.openDeleteModal();
    await this.closeDeleteModal(clickCancel);
  }

  async openEditModal(newName?: string, newExternalId?: number | string) {
    await this.rowLocator.locator('.mat-column-actions button').nth(0).click();
    if (newName) {
      await this.pageObj.modalPage.outerResourceEditNameInput().waitFor({ state: 'visible', timeout: 20000 });
      await this.pageObj.modalPage.outerResourceEditNameInput().fill(newName);
    }
    if (newExternalId) {
      await this.pageObj.modalPage.createOuterResourceExternalId().waitFor({ state: 'visible', timeout: 20000 });
      await this.pageObj.modalPage.createOuterResourceExternalId().fill(newExternalId.toString());
    }
  }

  async closeEditModal(clickCancel = false) {
    if (!clickCancel) {
      await this.pageObj.modalPage.outerResourceEditSaveBtn().click();
      await this.pageObj.page.locator('#spinner-animation').waitFor({ state: 'hidden', timeout: 20000 });
    } else {
      await this.pageObj.modalPage.outerResourceEditCancelBtn().click();
      await this.pageObj.newOuterResourceBtn().waitFor({ state: 'visible', timeout: 20000 });
    }
  }

  async edit(newName?: string, newExternalId?: number | string, clickCancel = false) {
    await this.openEditModal(newName, newExternalId);
    await this.closeEditModal(clickCancel);
  }
}
