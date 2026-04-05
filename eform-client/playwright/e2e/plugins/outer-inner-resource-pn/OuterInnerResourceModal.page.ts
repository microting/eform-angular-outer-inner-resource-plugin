import { Page, Locator } from '@playwright/test';

export class OuterInnerResourceModalPage {
  constructor(public page: Page) {}

  // Outer Resource - Create
  public outerResourceCreateNameInput(): Locator {
    return this.page.locator('#createOuterResourceName');
  }

  public outerResourceCreateSaveBtn(): Locator {
    return this.page.locator('#outerResourceCreateSaveBtn');
  }

  public outerResourceCreateCancelBtn(): Locator {
    return this.page.locator('#outerResourceCreateCancelBtn');
  }

  public createOuterResourceExternalId(): Locator {
    return this.page.locator('#createOuterResourceExternalId');
  }

  // Outer Resource - Edit
  public outerResourceEditNameInput(): Locator {
    return this.page.locator('#updateOuterResourceName');
  }

  public outerResourceEditSaveBtn(): Locator {
    return this.page.locator('#outerResourceEditSaveBtn');
  }

  public outerResourceEditCancelBtn(): Locator {
    return this.page.locator('#outerResourceEditCancelBtn');
  }

  // Outer Resource - Delete
  public outerResourceDeleteDeleteBtn(): Locator {
    return this.page.locator('#outerResourceDeleteDeleteBtn');
  }

  public outerResourceDeleteCancelBtn(): Locator {
    return this.page.locator('#outerResourceDeleteCancelBtn');
  }

  // Inner Resource - Create
  public innerResourceCreateNameInput(): Locator {
    return this.page.locator('#createInnerResourceName');
  }

  public innerResourceCreateSaveBtn(): Locator {
    return this.page.locator('#innerResourceCreateSaveBtn');
  }

  public innerResourceCreateCancelBtn(): Locator {
    return this.page.locator('#innerResourceCreateCancelBtn');
  }

  public createInnerResourceId(): Locator {
    return this.page.locator('#createInnerResourceId');
  }

  // Inner Resource - Edit
  public innerResourceEditName(): Locator {
    return this.page.locator('#updateInnerResourceName');
  }

  public innerResourceEditSaveBtn(): Locator {
    return this.page.locator('#innerResourceEditSaveBtn');
  }

  public innerResourceEditCancelBtn(): Locator {
    return this.page.locator('#innerResourceEditCancelBtn');
  }

  public innerResourceEditExternalIdInput(): Locator {
    return this.page.locator('#updateInnerResourceExternalId');
  }

  // Inner Resource - Delete
  public innerResourceDeleteDeleteBtn(): Locator {
    return this.page.locator('#innerResourceDeleteDeleteBtn');
  }

  public innerResourceDeleteCancelBtn(): Locator {
    return this.page.locator('#innerResourceDeleteCancelBtn');
  }
}
