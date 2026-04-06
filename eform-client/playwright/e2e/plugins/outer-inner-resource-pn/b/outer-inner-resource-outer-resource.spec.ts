import { test, expect } from '@playwright/test';
import { LoginPage } from '../../../Page objects/Login.page';
import { OuterInnerResourceOuterResourcePage } from '../OuterInnerResourceOuterResource.page';
import { OuterInnerResourceModalPage } from '../OuterInnerResourceModal.page';

let page;
let outerResourcePage: OuterInnerResourceOuterResourcePage;
let modalPage: OuterInnerResourceModalPage;

const newNameOuterResource = Math.random().toString(36).substring(7);
const nameForDeleteTest = Math.random().toString(36).substring(7);
const nameForEditTest = Math.random().toString(36).substring(7);

test.describe('Outer Inner Resource - Outer Resources', () => {
  test.beforeAll(async ({ browser }) => {
    page = await browser.newPage();
    outerResourcePage = new OuterInnerResourceOuterResourcePage(page);
    modalPage = new OuterInnerResourceModalPage(page);
    const loginPage = new LoginPage(page);
    await loginPage.open('/auth');
    await loginPage.login();
    await outerResourcePage.goToOuterResource();
  });

  test.afterAll(async () => {
    await page.close();
  });

  // Add tests
  test('should add outer resource with only name', async () => {
    const rowNumBeforeCreate = await outerResourcePage.rowNum();
    await outerResourcePage.createNewOuterResource(newNameOuterResource);
    expect(await outerResourcePage.rowNum()).toBe(rowNumBeforeCreate + 1);
    const listRowObject = await outerResourcePage.getOuterObjectByName(newNameOuterResource);
    expect(listRowObject).not.toBeNull();
    expect(listRowObject!.name).toBe(newNameOuterResource);
  });

  test('should not create outer resource without name', async () => {
    const rowNumBeforeCreate = await outerResourcePage.rowNum();
    await outerResourcePage.openCreateModal();
    expect(await modalPage.outerResourceCreateSaveBtn().isEnabled()).toBe(false);
    await outerResourcePage.closeCreateModal(true);
    expect(await outerResourcePage.rowNum()).toBe(rowNumBeforeCreate);
  });

  test('should clean up after add test', async () => {
    const listRowObject = await outerResourcePage.getOuterObjectByName(newNameOuterResource);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
  });

  // Edit tests
  test('should create outer resource for edit test', async () => {
    await outerResourcePage.createNewOuterResource(nameForEditTest);
    const listRowObject = await outerResourcePage.getOuterObjectByName(nameForEditTest);
    expect(listRowObject).not.toBeNull();
  });

  // TODO: Can't change name - edit test commented out in original
  // test('should edit outer resource', async () => {
  //   ...
  // });

  test('should clean up after edit test', async () => {
    const rowNumBeforeDelete = await outerResourcePage.rowNum();
    const listRowObject = await outerResourcePage.getOuterObjectByName(nameForEditTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
    expect(await outerResourcePage.rowNum()).toBe(rowNumBeforeDelete - 1);
  });

  // Delete tests
  test('should create outer resource for delete test', async () => {
    await outerResourcePage.createNewOuterResource(nameForDeleteTest);
    const listRowObject = await outerResourcePage.getOuterObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
  });

  test('should not delete outer resource when cancelling', async () => {
    const rowNumBeforeDelete = await outerResourcePage.rowNum();
    const listRowObject = await outerResourcePage.getOuterObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete(true);
    expect(await outerResourcePage.rowNum()).toBe(rowNumBeforeDelete);
  });

  test('should delete outer resource', async () => {
    const rowNumBeforeDelete = await outerResourcePage.rowNum();
    const listRowObject = await outerResourcePage.getOuterObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
    expect(await outerResourcePage.rowNum()).toBe(rowNumBeforeDelete - 1);
  });
});
