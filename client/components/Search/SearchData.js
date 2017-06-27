const SearchData = {
  region: {
    url: '/api/regions/search',
    label: 'Region',
    placeholder: 'RegionAdd',
    data: {},
  },
  activity: {
    url: '/api/activities/search',
    label: 'ActualMainActivity1',
    placeholder: 'RegMainActivity',
    data: {},
  },
  sectorCode: {
    url: '/api/sectorcodes/search',
    editUrl: '/api/sectorcodes/GetById/',
    label: 'InstSectorCode',
    placeholder: 'InstSectorCode',
    data: {},
  },
  legalForm: {
    url: '/api/legalforms/search',
    editUrl: '/api/legalforms/GetById/',
    label: 'LegalForm',
    placeholder: 'LegalForm',
    data: {},
  },
}

export default SearchData

