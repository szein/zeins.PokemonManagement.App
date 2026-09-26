import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CollectionService } from './collection-service';
import { Pokemon } from '../models/pokemon';
import { environment } from '../../environments/environment';

describe('CollectionService', () => {
  let service: CollectionService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CollectionService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('does not post a Pokemon that is already in the collection', () => {
    const pokemon: Pokemon = {
      id: 25,
      name: 'pikachu',
      height: 4,
      order: 25,
      weight: 60,
      url: 'pokemon/25',
    };
    let result: { alreadyExists: boolean } | undefined;

    service.addPokemonToCollection('collection-1', pokemon).subscribe(response => result = response);

    httpTestingController.expectOne(`${environment.apiUrl}/api/collection`).flush([{
      id: 'collection-1',
      name: 'My Collection',
      ownerId: 'owner-1',
      pokemons: [pokemon],
    }]);

    expect(result).toEqual({ alreadyExists: true });
    httpTestingController.expectNone(`${environment.apiUrl}/api/collection/collection-1/items`);
  });
});
