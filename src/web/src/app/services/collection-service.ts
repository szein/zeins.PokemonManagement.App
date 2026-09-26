import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pokemon } from '../models/pokemon';
import { environment } from '../../environments/environment';
import { CollectionModel } from '../models/collection';
import { hidden } from '@angular/forms/signals';
import { map, of, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class CollectionService {

    private collectionId!: string;
    constructor (private http: HttpClient){}

    getCollectionPokemons() {
        return this.http.get<CollectionModel[]>(
            `${environment.apiUrl}/api/collection`
        )
    }

    addPokemonToCollection(
        collectionId: string,
        pokemon: Pokemon
    ){
        return this.getCollectionPokemons().pipe(
            switchMap((collections) => {
                const collection = collections.find(item => item.id === collectionId);
                const alreadyExists = collection?.pokemons.some(item => item.id === pokemon.id) ?? false;

                if (alreadyExists) {
                    return of({ alreadyExists: true });
                }

                return this.http.post(
                    `${environment.apiUrl}/api/collection/${collectionId}/items`,
                    {
                        "id": pokemon.id,
                        "name": pokemon.name,
                        "height": pokemon.height,
                        "weight": pokemon.weight,
                        "url": pokemon.url
                    }
                ).pipe(map(() => ({ alreadyExists: false })));
            })
        );
    }

    removePokemonFromCollection(
        collectionId: string,
        itemId: string
    ){
        return this.http.delete(
        `${environment.apiUrl}/api/collection/${collectionId}/items/${itemId}`
        )
    }
}