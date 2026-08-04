export interface CountryLocation {
  name: string;
  code: string;
  dialCode: string;
  flag: string;
  flagUrl: string;
  states: string[];
  cities: string[];
  postalFormatHint: string;
}

export const COUNTRY_DATA: CountryLocation[] = [
  {
    name: 'United States',
    code: 'US',
    dialCode: '+1',
    flag: '🇺🇸',
    flagUrl: 'https://flagcdn.com/w40/us.png',
    states: ['California', 'New York', 'Texas', 'Florida', 'Illinois', 'Washington', 'Massachusetts', 'Georgia'],
    cities: ['San Francisco', 'New York', 'Los Angeles', 'Chicago', 'Houston', 'Miami', 'Seattle', 'Austin', 'Boston', 'Dallas'],
    postalFormatHint: 'e.g. 94107 or 10001'
  },
  {
    name: 'Pakistan',
    code: 'PK',
    dialCode: '+92',
    flag: '🇵🇰',
    flagUrl: 'https://flagcdn.com/w40/pk.png',
    states: ['Sindh', 'Punjab', 'Khyber Pakhtunkhwa', 'Balochistan', 'Islamabad Capital Territory', 'Azad Kashmir'],
    cities: ['Karachi', 'Lahore', 'Islamabad', 'Rawalpindi', 'Faisalabad', 'Peshawar', 'Quetta', 'Multan', 'Sialkot', 'Gujranwala', 'Hyderabad'],
    postalFormatHint: 'e.g. 75500 or 44000'
  },
  {
    name: 'United Kingdom',
    code: 'GB',
    dialCode: '+44',
    flag: '🇬🇧',
    flagUrl: 'https://flagcdn.com/w40/gb.png',
    states: ['England', 'Scotland', 'Wales', 'Northern Ireland'],
    cities: ['London', 'Manchester', 'Birmingham', 'Edinburgh', 'Glasgow', 'Leeds', 'Liverpool', 'Bristol'],
    postalFormatHint: 'e.g. EC1A 1BB or M1 1AE'
  },
  {
    name: 'United Arab Emirates',
    code: 'AE',
    dialCode: '+971',
    flag: '🇦🇪',
    flagUrl: 'https://flagcdn.com/w40/ae.png',
    states: ['Dubai', 'Abu Dhabi', 'Sharjah', 'Ajman', 'Ras Al Khaimah', 'Fujairah', 'Umm Al Quwain'],
    cities: ['Dubai', 'Abu Dhabi', 'Sharjah', 'Ajman', 'Ras Al Khaimah', 'Al Ain'],
    postalFormatHint: 'e.g. 00000 or P.O. Box 12345'
  },
  {
    name: 'Saudi Arabia',
    code: 'SA',
    dialCode: '+966',
    flag: '🇸🇦',
    flagUrl: 'https://flagcdn.com/w40/sa.png',
    states: ['Riyadh Region', 'Makkah Region', 'Eastern Province', 'Madinah Region', 'Asir Region'],
    cities: ['Riyadh', 'Jeddah', 'Mecca', 'Medina', 'Dammam', 'Khobar', 'Dhahran'],
    postalFormatHint: 'e.g. 11564 or 21421'
  },
  {
    name: 'Canada',
    code: 'CA',
    dialCode: '+1',
    flag: '🇨🇦',
    flagUrl: 'https://flagcdn.com/w40/ca.png',
    states: ['Ontario', 'Quebec', 'British Columbia', 'Alberta', 'Manitoba', 'Nova Scotia'],
    cities: ['Toronto', 'Vancouver', 'Montreal', 'Calgary', 'Ottawa', 'Edmonton', 'Quebec City'],
    postalFormatHint: 'e.g. M5H 2N2 or V6B 1A1'
  },
  {
    name: 'Australia',
    code: 'AU',
    dialCode: '+61',
    flag: '🇦🇺',
    flagUrl: 'https://flagcdn.com/w40/au.png',
    states: ['New South Wales', 'Victoria', 'Queensland', 'Western Australia', 'South Australia'],
    cities: ['Sydney', 'Melbourne', 'Brisbane', 'Perth', 'Adelaide', 'Gold Coast', 'Canberra'],
    postalFormatHint: 'e.g. 2000 or 3000'
  },
  {
    name: 'Germany',
    code: 'DE',
    dialCode: '+49',
    flag: '🇩🇪',
    flagUrl: 'https://flagcdn.com/w40/de.png',
    states: ['Bavaria', 'North Rhine-Westphalia', 'Baden-Württemberg', 'Berlin', 'Hesse', 'Hamburg'],
    cities: ['Berlin', 'Munich', 'Frankfurt', 'Hamburg', 'Cologne', 'Stuttgart', 'Düsseldorf'],
    postalFormatHint: 'e.g. 10115 or 80331'
  },
  {
    name: 'India',
    code: 'IN',
    dialCode: '+91',
    flag: '🇮🇳',
    flagUrl: 'https://flagcdn.com/w40/in.png',
    states: ['Maharashtra', 'Delhi NCR', 'Karnataka', 'Telangana', 'Tamil Nadu', 'Gujarat', 'West Bengal'],
    cities: ['Mumbai', 'Delhi', 'Bangalore', 'Hyderabad', 'Chennai', 'Kolkata', 'Pune', 'Ahmedabad'],
    postalFormatHint: 'e.g. 400001 or 110001'
  },
  {
    name: 'France',
    code: 'FR',
    dialCode: '+33',
    flag: '🇫🇷',
    flagUrl: 'https://flagcdn.com/w40/fr.png',
    states: ['Île-de-France', 'Auvergne-Rhône-Alpes', 'Provence-Alpes-Côte d\'Azur', 'Nouvelle-Aquitaine'],
    cities: ['Paris', 'Marseille', 'Lyon', 'Toulouse', 'Nice', 'Nantes', 'Bordeaux', 'Strasbourg'],
    postalFormatHint: 'e.g. 75001 or 13001'
  },
  {
    name: 'Singapore',
    code: 'SG',
    dialCode: '+65',
    flag: '🇸🇬',
    flagUrl: 'https://flagcdn.com/w40/sg.png',
    states: ['Central Region', 'East Region', 'North Region', 'West Region', 'North-East Region'],
    cities: ['Singapore', 'Jurong', 'Woodlands', 'Tampines', 'Bedok', 'Ang Mo Kio'],
    postalFormatHint: 'e.g. 018989 or 238801'
  },
  {
    name: 'Turkey',
    code: 'TR',
    dialCode: '+90',
    flag: '🇹🇷',
    flagUrl: 'https://flagcdn.com/w40/tr.png',
    states: ['Marmara', 'Central Anatolia', 'Aegean', 'Mediterranean'],
    cities: ['Istanbul', 'Ankara', 'Izmir', 'Bursa', 'Antalya', 'Adana', 'Gaziantep'],
    postalFormatHint: 'e.g. 34000 or 06000'
  },
  {
    name: 'Japan',
    code: 'JP',
    dialCode: '+81',
    flag: '🇯🇵',
    flagUrl: 'https://flagcdn.com/w40/jp.png',
    states: ['Tokyo Metropolis', 'Osaka Prefecture', 'Kanagawa', 'Aichi', 'Hokkaido', 'Fukuoka'],
    cities: ['Tokyo', 'Osaka', 'Yokohama', 'Nagoya', 'Sapporo', 'Kobe', 'Kyoto'],
    postalFormatHint: 'e.g. 100-0001 or 530-0001'
  },
  {
    name: 'China',
    code: 'CN',
    dialCode: '+86',
    flag: '🇨🇳',
    flagUrl: 'https://flagcdn.com/w40/cn.png',
    states: ['Guangdong', 'Zhejiang', 'Jiangsu', 'Beijing', 'Shanghai', 'Shandong'],
    cities: ['Shanghai', 'Beijing', 'Shenzhen', 'Guangzhou', 'Hangzhou', 'Chengdu'],
    postalFormatHint: 'e.g. 200000 or 100000'
  },
  {
    name: 'Brazil',
    code: 'BR',
    dialCode: '+55',
    flag: '🇧🇷',
    flagUrl: 'https://flagcdn.com/w40/br.png',
    states: ['São Paulo', 'Rio de Janeiro', 'Minas Gerais', 'Bahia', 'Paraná'],
    cities: ['São Paulo', 'Rio de Janeiro', 'Brasília', 'Salvador', 'Fortaleza', 'Belo Horizonte'],
    postalFormatHint: 'e.g. 01000-000 or 20000-000'
  },
  {
    name: 'South Africa',
    code: 'ZA',
    dialCode: '+27',
    flag: '🇿🇦',
    flagUrl: 'https://flagcdn.com/w40/za.png',
    states: ['Gauteng', 'Western Cape', 'KwaZulu-Natal', 'Eastern Cape'],
    cities: ['Johannesburg', 'Cape Town', 'Durban', 'Pretoria', 'Port Elizabeth'],
    postalFormatHint: 'e.g. 2000 or 8000'
  }
];
