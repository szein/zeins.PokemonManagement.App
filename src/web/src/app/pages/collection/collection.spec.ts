import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Collection } from './collection';
import { CollectionService } from '../../services/collection-service';
import { CollectionStateService } from '../../state/collection-state.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Pokemon } from '../../models/pokemon';

describe('Collection', () => {
  let component: Collection;
  let fixture: ComponentFixture<Collection>;
  let collectionRequestCount: number;

  beforeEach(async () => {
    const pokemons: Pokemon[] = Array.from({ length: 12 }, (_, index) => ({
      id: index + 1,
      name: `Pokemon ${index + 1}`,
      height: 1,
      order: index + 1,
      weight: 1,
      url: ''
    }));
    collectionRequestCount = 0;

    await TestBed.configureTestingModule({
      imports: [Collection],
      providers: [
        {
          provide: CollectionService,
          useValue: {
            getCollectionPokemons: () => {
              collectionRequestCount++;
              return of([{ id: 'collection-id', name: 'Default', ownerId: 'owner-id', pokemons }]);
            },
            removePokemonFromCollection: () => of(null)
          }
        },
        { provide: CollectionStateService, useValue: { setCollection: () => undefined } },
        { provide: MatSnackBar, useValue: { open: () => undefined } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Collection);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should paginate the loaded collection without requesting another page', () => {
    expect(component.totalItems).toBe(12);
    expect(component.pokemons.map(pokemon => pokemon.id)).toEqual([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);

    component.onPageChange({
      pageIndex: 1,
      pageSize: 5,
      length: 12,
      previousPageIndex: 0
    });

    expect(component.pokemons.map(pokemon => pokemon.id)).toEqual([6, 7, 8, 9, 10]);
    expect(collectionRequestCount).toBe(1);
  });
});
