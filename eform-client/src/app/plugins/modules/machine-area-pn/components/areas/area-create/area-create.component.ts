import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-area-create',
  templateUrl: './area-create.component.html',
  styleUrls: ['./area-create.component.scss']
})
export class AreaCreateComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onNewAreaCreated: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  newAreaModel: {
    name: ''
  };
  constructor() { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  createArea() {

  }
}
