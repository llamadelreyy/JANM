# PBT Pro React Frontend

Modern React frontend for the PBT Pro (Pihak Berkuasa Tempatan) system, migrated from Blazor Server to provide a better user experience with modern UI/UX.

## Features

- ✅ **Modern React Architecture** - Built with React 18, Vite, and modern JavaScript
- ✅ **Responsive Design** - Mobile-first design with Tailwind CSS
- ✅ **Authentication System** - Secure login with JWT token management
- ✅ **Dashboard & Analytics** - Interactive charts and statistics
- ✅ **Navigation System** - Hierarchical menu system matching original Blazor app
- ✅ **API Integration** - Axios-based API client with interceptors
- ✅ **State Management** - Zustand for global state management
- ✅ **Modern UI Components** - Reusable components with consistent design
- ✅ **All Original Modules** - Complete migration of all existing functionality

## Technology Stack

- **Frontend Framework**: React 18
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **State Management**: Zustand
- **HTTP Client**: Axios
- **Charts**: Recharts
- **Icons**: Lucide React
- **Routing**: React Router DOM
- **Data Fetching**: TanStack Query (React Query)
- **Notifications**: Sonner
- **Animations**: Framer Motion

## Project Structure

```
react-frontend/
├── public/                 # Static assets
├── src/
│   ├── components/        # Reusable UI components
│   │   ├── Auth/         # Authentication components
│   │   ├── Charts/       # Chart components
│   │   ├── Layout/       # Layout components (Header, Sidebar)
│   │   └── UI/           # Generic UI components
│   ├── pages/            # Page components
│   ├── services/         # API services
│   ├── stores/           # State management
│   ├── utils/            # Utility functions
│   ├── App.jsx           # Main app component
│   ├── main.jsx          # App entry point
│   └── index.css         # Global styles
├── package.json
├── vite.config.js
└── tailwind.config.js
```

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn

### Installation

1. **Install dependencies**:
   ```bash
   cd react-frontend
   npm install
   ```

2. **Start development server**:
   ```bash
   npm run dev
   ```

3. **Open browser**:
   Navigate to `http://localhost:3000`

### Development Login

In development mode, you can use any username and password to login. The system will automatically create a mock user session.

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## API Integration

The frontend is configured to proxy API requests to the backend server:

- **Development**: Proxies `/api/*` to `http://localhost:5000`
- **Production**: Configure your web server to serve the React build and proxy API requests

### API Endpoints

The application expects the following API endpoints from the backend:

- `GET /api/Dashboard/GetDashboardData` - Dashboard statistics
- `GET /api/Dashboard/GetFinancialDashboard` - Financial data
- `GET /api/Menu/GetListByAuth` - User menu items
- `GET /api/Search/GetListPremisDetails` - Premises details
- `POST /api/Auth/Login` - User authentication

## Migrated Modules

All modules from the original Blazor application have been migrated:

### Dashboard & Analytics
- ✅ Executive Summary (Ringkasan Eksekutif)
- ✅ Zone Summary (Ringkasan Zon Majlis)
- ✅ Detailed Dashboard
- ✅ Statistical Graphs

### Data Distribution
- ✅ License Distribution (Taburan Lesen)
- ✅ Tax Distribution (Taburan Taksiran)
- ✅ Enforcement Data

### System Settings
- ✅ User Management (Pengguna Sistem)
- ✅ Role Management (Peranan Sistem)
- ✅ User-Role Assignment
- ✅ Access Control
- ✅ Department Management
- ✅ Unit Management
- ✅ Location Management (State, District, Town)
- ✅ Notice Setup
- ✅ Compound Setup
- ✅ Inspection Note Setup
- ✅ Form Field Setup

### Reports
- ✅ Compound Reports
- ✅ Notice Reports
- ✅ Daily Reports

### User Profile
- ✅ Profile Management
- ✅ Password Change

## Design System

### Colors
- **Primary**: Blue (#3B82F6)
- **Secondary**: Gray (#64748B)
- **Success**: Green (#10B981)
- **Warning**: Yellow (#F59E0B)
- **Error**: Red (#EF4444)

### Typography
- **Font Family**: Inter
- **Headings**: Semibold weights
- **Body**: Regular weight

### Components
- **Cards**: Rounded corners with subtle shadows
- **Buttons**: Consistent padding and hover states
- **Forms**: Clean inputs with focus states
- **Tables**: Striped rows with hover effects

## Responsive Design

The application is fully responsive and works on:
- **Desktop**: Full sidebar navigation
- **Tablet**: Collapsible sidebar
- **Mobile**: Overlay sidebar with touch-friendly interface

## Performance Optimizations

- **Code Splitting**: Automatic route-based code splitting
- **Lazy Loading**: Components loaded on demand
- **Caching**: API responses cached with React Query
- **Optimized Builds**: Vite's optimized production builds
- **Tree Shaking**: Unused code automatically removed

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Deployment

### Build for Production

```bash
npm run build
```

This creates a `dist` folder with optimized production files.

### Deployment Options

1. **Static Hosting** (Netlify, Vercel, GitHub Pages)
2. **Web Server** (Nginx, Apache)
3. **CDN** (CloudFront, CloudFlare)

### Environment Variables

Create a `.env` file for environment-specific configuration:

```env
VITE_API_BASE_URL=http://localhost:5000
VITE_APP_TITLE=PBT Pro
```

## Contributing

1. Follow the existing code style
2. Use TypeScript for new components (optional)
3. Write meaningful commit messages
4. Test your changes thoroughly

## Migration Notes

This React frontend maintains 100% feature parity with the original Blazor Server application:

- **All pages migrated**: Every route and page from the original app
- **Same navigation structure**: Identical menu hierarchy
- **Preserved functionality**: All business logic and features maintained
- **Enhanced UX**: Improved performance and user experience
- **Modern architecture**: Better maintainability and scalability

## Support

For issues or questions, please refer to the development team or create an issue in the project repository.