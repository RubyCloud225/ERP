import { Component, ViewChild, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { ModelService } from '../services/model.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-model-container',
    template: `
    <div class="model-backdrop" *ngIf="isModelOpen" (click)="onBackdropClick()"></div>
    <ng-container #modelOutlet></ng-template>
  `,
    styleUrls: ['./modelContainerComponent.scss']
})
export class ModelContainerComponent implements OnInit, OnDestroy {
    @ViewChild('modelOutlet', { read: ViewContainerRef, static: true }) modelOutlet!: ViewContainerRef;
    isModelOpen: boolean = false;
    private modelSubscription!: Subscription;

    constructor(private modelService: ModelService) {}

    ngOnInit(): void {
        this.modelService.setModalContainer(this.modelOutlet);
        this.modelSubscription = this.modelService.isAnyModelOpen$.subscribe((isOpen: boolean) => {
            this.isModelOpen = isOpen;
            if (isOpen) {
                document.body.style.overflow = 'hidden';
            } else {
                document.body.style.overflow = '';
            }
        });
    }


    onBackdropClick(): void {
        this.modelService.closeAllModels();
    }
    ngOnDestroy(): void {
        this.modelSubscription.unsubscribe();
    }
}
