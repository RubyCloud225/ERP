import { Injectable, Type, ComponentRef, ViewContainerRef, EnvironmentInjector, inject } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { take } from 'rxjs/operators';

// Define the interface for the model comtent
export interface ModelContent<T = any, R = any> {
    data?: T;
    close: (result?: R) => void;
}

@Injectable({
    providedIn: 'root'
})
export class ModelService {
    private _isAnyModelOpen = new BehaviorSubject<boolean>(false);
    public readonly isAnyModelOpen$: Observable<boolean> = this._isAnyModelOpen.asObservable();
    // dynamic component reference
    private vcr!: ViewContainerRef;
    private environmentInjector = inject(EnvironmentInjector);
    // emit results from opened modal
    private _modelResultSubject = new Subject<any>();
    constructor() {}
    // Open a model with the given component and data
    setModalContainer(vcr: ViewContainerRef): void {
        this.vcr = vcr;
    }
    /**
     * Open a modal with the given component and data.
     * @param component - The component to be displayed in the modal.
     * @param data - The data to be passed to the modal component.
     * @return A promise that resolves with the result from the modal.
     */
    openModal<T, R>(componentType: Type<ModelContent<T, R>>, data?: T): Observable<R | undefined> {
        if (!this.vcr) {
            console.error('No view container reference provided');
            return new Observable<R | undefined>(observer => observer.complete()); // Return an empty observable
        }
        this.closeAllModels(); // Close any existing modals
        this._modelResultSubject = new Subject<R | undefined>(); // Create a new subject for the modal result
        this.vcr.clear(); // Clear the view container reference

        // Create a component reference for the modal
        const componentRef: ComponentRef<ModelContent<T, R>> = this.vcr.createComponent(componentType, {
            environmentInjector: this.environmentInjector // Use the environment injector to create the component
        });
        // Pass the data to the modal component
        if (data) {
            componentRef.instance.data = data;
        }
        // close function to the model component
        componentRef.instance.close = (result?: R) => {
            this.close(componentRef, result);
        };
        // Set the modal open state
        this._isAnyModelOpen.next(true);
        return this._modelResultSubject.asObservable().pipe(take(1)); // Return an observable that emits the result once
    }
    /**
     * Close the modal and emit the result.
     * @param componentRef - The component reference of the modal to be closed.
     * @param result - The result to be emitted when the modal is closed.
     * */
    close<R>(componentRef: ComponentRef<ModelContent<any, R>>, result?: R): void {
        if (componentRef) {
            componentRef.destroy(); // Destroy the component reference to close the modal
        }
        this._modelResultSubject.next(result); // Emit the result to subscribers
        this._modelResultSubject.complete(); // Complete the subject
        this._isAnyModelOpen.next(false); // Set the modal open state to false
    }
    /**
     * Close all open modals.
     */
    closeAllModels(): void {
        if (this.vcr) {
            this.vcr.clear(); // Clear the view container reference to close all modals
            this._isAnyModelOpen.next(false); // Set the modal open state to false
            this._modelResultSubject.complete(); // Complete the subject to notify subscribers
            this._modelResultSubject = new Subject<any>(); // Reset the subject for future use
            this._modelResultSubject.next(undefined); // Emit undefined to indicate that no result is available
        }
    }
}