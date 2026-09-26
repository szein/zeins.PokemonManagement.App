import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";

@Injectable({
  providedIn: 'root'
})
export class PokemonStateService {

  private pokemonCountSubject =
    new BehaviorSubject<number | null>(null);

  storeCount$ =
    this.pokemonCountSubject.asObservable();

  setStoreCount(count: number) {
    this.pokemonCountSubject.next(count);
  }

  get storeCount() {
    return this.pokemonCountSubject.value;
  }
}