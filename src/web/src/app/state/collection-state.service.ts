import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";

@Injectable({
  providedIn: 'root'
})
export class CollectionStateService {

  private collectionIdSubject =
    new BehaviorSubject<string | null>(null);

  collectionId$ =
    this.collectionIdSubject.asObservable();

  setCollection(id: string) {
    this.collectionIdSubject.next(id);
  }

  get collectionId() {
    return this.collectionIdSubject.value;
  }

  private collectionCountSubject =
    new BehaviorSubject<number | null>(null);

  collectionCount$ =
    this.collectionCountSubject.asObservable();

  setCollectionCount(count: number) {
    this.collectionCountSubject.next(count);    
  }

  get collectionCount() {
    return this.collectionCountSubject.value;
  }
}