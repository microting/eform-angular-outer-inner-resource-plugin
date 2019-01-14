import {ChangeDetectorRef, Component, EventEmitter, OnInit} from '@angular/core';
import { MachineAreaPnSettingsService} from '../../services';
import {Router} from '@angular/router';
import {MachineAreaSettingsModel} from '../../models';
import {debounceTime, switchMap} from 'rxjs/operators';
import {EntitySearchService} from '../../../../../common/services/advanced';
import {TemplateListModel, TemplateRequestModel} from '../../../../../common/models/eforms';
import {EFormService} from '../../../../../common/services/eform';

@Component({
  selector: 'app-machine-area-settings',
  templateUrl: './machine-area-settings.component.html',
  styleUrls: ['./machine-area-settings.component.scss']
})
export class MachineAreaSettingsComponent implements OnInit {
  spinnerStatus = false;
  typeahead = new EventEmitter<string>();
  machineAreaPnsettingsModel: MachineAreaSettingsModel = new MachineAreaSettingsModel();
  templatesModel: TemplateListModel = new TemplateListModel();
  templateRequestModel: TemplateRequestModel = new TemplateRequestModel();

  constructor(
    private machineAreaSettingsService: MachineAreaPnSettingsService,
    private router: Router,
    private eFormService: EFormService,
    private entitySearchService: EntitySearchService,
    private cd: ChangeDetectorRef) {
    this.typeahead
      .pipe(
        debounceTime(200),
        switchMap(term => {
          this.templateRequestModel.nameFilter = term;
          return this.eFormService.getAll(this.templateRequestModel);
        })
      )
      .subscribe(items => {
        this.templatesModel = items.model;
        this.cd.markForCheck();
      });
  }

  ngOnInit() {
    this.getSettings();
  }

  getSettings() {
    debugger;
    this.spinnerStatus = true;
    this.machineAreaSettingsService.getAllSettings().subscribe((data) => {
      if (data && data.success) {
        this.machineAreaPnsettingsModel = data.model;
      } this.spinnerStatus = false;
    });
  }
  updateSettings() {
    debugger;
    this.spinnerStatus = true;
    this.machineAreaSettingsService.updateSettings(this.machineAreaPnsettingsModel).subscribe((data) => {
      if (data && data.success) {
        this.spinnerStatus = true;
        //     this.router.navigate(['/plugins/machine-area-pn']).then();
      } this.spinnerStatus = false;
    });
  }
  onSelectedChanged(e: any) {
    // debugger;
    this.machineAreaPnsettingsModel.selectedTemplateId = e.id;
  }
}
