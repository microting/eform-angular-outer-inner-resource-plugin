import { test, expect } from '@playwright/test';
import { LoginPage } from '../../../Page objects/Login.page';
import { OuterInnerResourceInnerResourcePage } from '../OuterInnerResourceInnerResource.page';
import { OuterInnerResourceModalPage } from '../OuterInnerResourceModal.page';

let page;
let innerResourcePage: OuterInnerResourceInnerResourcePage;
let modalPage: OuterInnerResourceModalPage;

const newNameInnerResource = Math.random().toString(36).substring(7);
const nameForDeleteTest = Math.random().toString(36).substring(7);
const nameForEditTest = Math.random().toString(36).substring(7);

test.describe('Outer Inner Resource - Inner Resources', () => {
  test.beforeAll(async ({ browser }) => {
    page = await browser.newPage();
    innerResourcePage = new OuterInnerResourceInnerResourcePage(page);
    modalPage = new OuterInnerResourceModalPage(page);
    const loginPage = new LoginPage(page);
    await loginPage.open('/auth');
    await loginPage.login();
    await innerResourcePage.goToInnerResource();
  });

  test.afterAll(async () => {
    await page.close();
  });

  // Add tests
  test('should not create a new inner resource without name', async () => {
    const rowNumBeforeCreate = await innerResourcePage.rowNum();
    await innerResourcePage.openCreateModal();
    expect(await modalPage.innerResourceCreateSaveBtn().isEnabled()).toBe(false);
    await innerResourcePage.closeCreateModal(true);
    expect(await innerResourcePage.rowNum()).toBe(rowNumBeforeCreate);
  });

  test('should add inner resource with only name', async () => {
    const rowNumBeforeCreate = await innerResourcePage.rowNum();
    await innerResourcePage.createNewInnerResource(newNameInnerResource);
    expect(await innerResourcePage.rowNum()).toBe(rowNumBeforeCreate + 1);
    const listRowObject = await innerResourcePage.getInnerObjectByName(newNameInnerResource);
    expect(listRowObject).not.toBeNull();
    expect(listRowObject!.name).toBe(newNameInnerResource);
  });

  test('should clean up after add test', async () => {
    const listRowObject = await innerResourcePage.getInnerObjectByName(newNameInnerResource);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
  });

  // Edit tests
  test('should create inner resource for edit test', async () => {
    await innerResourcePage.createNewInnerResource(nameForEditTest);
    const listRowObject = await innerResourcePage.getInnerObjectByName(nameForEditTest);
    expect(listRowObject).not.toBeNull();
  });

  // TODO: Can't change name - edit test commented out in original
  // test('should edit inner resource', async () => {
  //   ...
  // });

  test('should clean up after edit test', async () => {
    const listRowObject = await innerResourcePage.getInnerObjectByName(nameForEditTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
  });

  // Delete tests
  test('should create inner resource for delete test', async () => {
    await innerResourcePage.createNewInnerResource(nameForDeleteTest);
    const listRowObject = await innerResourcePage.getInnerObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
  });

  test('should not delete inner resource when cancelling', async () => {
    const rowNumBeforeDelete = await innerResourcePage.rowNum();
    const listRowObject = await innerResourcePage.getInnerObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete(true);
    expect(await innerResourcePage.rowNum()).toBe(rowNumBeforeDelete);
  });

  test('should delete inner resource', async () => {
    const rowNumBeforeDelete = await innerResourcePage.rowNum();
    const listRowObject = await innerResourcePage.getInnerObjectByName(nameForDeleteTest);
    expect(listRowObject).not.toBeNull();
    await listRowObject!.delete();
    expect(await innerResourcePage.rowNum()).toBe(rowNumBeforeDelete - 1);
  });
});
