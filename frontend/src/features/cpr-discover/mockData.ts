export interface Station {
  id: string
  name: string
  address: string
  distanceKm: number
  slotsAvailable: number
  nextSlot: string
  certifications: string[]
  rating: number
  gradientFrom: string
  gradientTo: string
}

export const STATIONS: Station[] = [
  { id: '1', name: 'Copenhagen Central Training Hub', address: 'Vesterbrogade 12, 1620 Copenhagen', distanceKm: 0.4, slotsAvailable: 3, nextSlot: 'Tomorrow 09:00', certifications: ['BLS', 'AED'], rating: 4.9, gradientFrom: '#dbeafe', gradientTo: '#e0f2fe' },
  { id: '2', name: 'Rigshospitalet Training Center',  address: 'Blegdamsvej 9, 2100 Copenhagen',   distanceKm: 1.2, slotsAvailable: 8, nextSlot: 'Today 14:30',    certifications: ['AED'],        rating: 4.7, gradientFrom: '#fef3c7', gradientTo: '#fde68a' },
  { id: '3', name: 'Nørrebro CPR Station',            address: 'Nørrebrogade 43, 2200 Copenhagen', distanceKm: 2.1, slotsAvailable: 0, nextSlot: 'Jun 3rd',         certifications: ['BLS'],        rating: 4.8, gradientFrom: '#fce7f3', gradientTo: '#fbcfe8' },
  { id: '4', name: 'Østerbro Skills Lab',             address: 'Østerbrogade 102, 2100 Copenhagen',distanceKm: 3.4, slotsAvailable: 12,nextSlot: 'Today 16:00',    certifications: ['BLS'],        rating: 4.6, gradientFrom: '#e0e7ff', gradientTo: '#c7d2fe' },
  { id: '5', name: 'Frederiksberg CPR Hub',           address: 'Smallegade 2, 2000 Frederiksberg', distanceKm: 4.1, slotsAvailable: 5, nextSlot: 'Tomorrow 11:00', certifications: ['BLS', 'AED'], rating: 4.5, gradientFrom: '#dcfce7', gradientTo: '#bbf7d0' },
  { id: '6', name: 'Bispebjerg Hospital Lab',         address: 'Bispebjerg Bakke 23, 2400 Copenhagen', distanceKm: 5.0, slotsAvailable: 2, nextSlot: 'Jun 4th', certifications: ['BLS'], rating: 4.4, gradientFrom: '#f3e8ff', gradientTo: '#e9d5ff' },
]
