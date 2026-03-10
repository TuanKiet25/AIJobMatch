## ?? Admin Dashboard API Documentation

### Base URL
```
https://localhost:7145/api/admin-dashboard
```

### Authentication
T?t c? endpoints yêu c?u:
- **Header:** `Authorization: Bearer {jwt_token}`
- **Role:** `Admin`

---

### 1. **GET /overview** - T?ng quan Dashboard
L?y toàn b? thông tin t?ng quan v? doanh thu, giao d?ch, user, v.v.

**Response:**
```json
{
  "success": true,
  "data": {
    "totalRevenue": 50000000,
    "totalTransactions": 150,
    "totalSubscribedUsers": 120,
    "totalSubscriptionsSold": 180,
    "thisMonthRevenue": 8500000,
    "thisMonthTransactions": 25,
    "lastMonthRevenue": 7200000,
    "growthPercentage": 18.06,
    "newUsersThisMonth": 45
  }
}
```

---

### 2. **GET /revenue-summary** - Tóm t?t Doanh thu
L?y tóm t?t doanh thu trong kho?ng th?i gian c? th?.

**Query Parameters:**
- `startDate` (optional): DateTime - Ngày b?t ??u (default: 1 tháng tr??c)
- `endDate` (optional): DateTime - Ngày k?t thúc (default: hôm nay)

**Example:**
```
GET /revenue-summary?startDate=2025-01-01&endDate=2025-12-31
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalRevenue": 45000000,
    "completedTransactions": 140,
    "averageTransactionAmount": 321428.57,
    "periodStart": "2025-01-01T00:00:00Z",
    "periodEnd": "2025-12-31T23:59:59Z"
  }
}
```

---

### 3. **GET /subscription-sales** - Th?ng kê bán hàng theo gói
L?y chi ti?t v? bán hàng c?a t?ng gói subscription.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "planId": "guid-uuid",
      "planName": "Pro",
      "planPrice": 299000,
      "totalSold": 85,
      "totalRevenue": 25415000,
      "percentageOfTotal": 50.83,
      "activeSubscriptions": 60,
      "expiredSubscriptions": 25
    },
    {
      "planId": "guid-uuid",
      "planName": "Plus",
      "planPrice": 99000,
      "totalSold": 120,
      "totalRevenue": 11880000,
      "percentageOfTotal": 23.76,
      "activeSubscriptions": 90,
      "expiredSubscriptions": 30
    }
  ]
}
```

---

### 4. **GET /top-revenue-months** - Tháng có doanh thu cao nh?t
L?y danh sách các tháng có doanh thu cao nh?t.

**Query Parameters:**
- `monthCount` (optional): int - S? tháng c?n l?y (default: 12)

**Example:**
```
GET /top-revenue-months?monthCount=6
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "month": "2025-12-01T00:00:00Z",
      "revenue": 9500000,
      "transactionCount": 32
    },
    {
      "month": "2025-11-01T00:00:00Z",
      "revenue": 8700000,
      "transactionCount": 28
    }
  ]
}
```

---

### 5. **GET /user-statistics** - Th?ng kê User
L?y th?ng kê v? user, bao g?m t?ng s? user, number of candidates/recruiters, subscription conversion rate.

**Response:**
```json
{
  "success": true,
  "data": {
    "totalUsers": 500,
    "candidateUsers": 300,
    "recruiterUsers": 200,
    "usersWithActiveSubscription": 120,
    "usersWithExpiredSubscription": 60,
    "subscriptionConversionRate": 36.0
  }
}
```

---

### 6. **GET /recent-transactions** - Giao d?ch g?n ?ây
L?y danh sách các giao d?ch g?n ?ây nh?t.

**Query Parameters:**
- `take` (optional): int - S? giao d?ch c?n l?y (default: 10)

**Example:**
```
GET /recent-transactions?take=20
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "transactionId": "guid-uuid",
      "userEmail": "user@example.com",
      "planName": "Pro",
      "amount": 299000,
      "status": "Completed",
      "createdDate": "2025-12-15T10:30:00Z"
    },
    {
      "transactionId": "guid-uuid",
      "userEmail": "user2@example.com",
      "planName": "Plus",
      "amount": 99000,
      "status": "Completed",
      "createdDate": "2025-12-14T15:45:00Z"
    }
  ]
}
```

---

## ?? Test v?i Postman

### 1. L?y JWT Token
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "password",
  "captchaToken": "token"
}
```

### 2. S? d?ng token ?? access Dashboard
```bash
GET /api/admin-dashboard/overview
Authorization: Bearer {jwt_token}
```

---

## ?? Các metrics ???c cung c?p:

### Dashboard Overview
- ? T?ng doanh thu
- ? T?ng s? giao d?ch thành công
- ? T?ng s? user ?ã mua subscription
- ? S? gói subscription ?ã bán
- ? Doanh thu tháng này
- ? S? giao d?ch tháng này
- ? Doanh thu tháng tr??c
- ? T? l? t?ng tr??ng (%)
- ? S? user m?i tháng này

### Subscription Sales
- ? S? l??ng bán t?ng gói
- ? Doanh thu t?ng gói
- ? T? l? bán hàng (%)
- ? S? subscription ?ang active
- ? S? subscription ?ã h?t h?n

### User Statistics
- ? T?ng s? user
- ? Phân chia: Candidates vs Recruiters
- ? User v?i subscription active
- ? Conversion rate (%)

---

## ? Error Handling

### Unauthorized (401)
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### Forbidden (403)
```json
{
  "success": false,
  "message": "You don't have permission to access this resource"
}
```

### Bad Request (400)
```json
{
  "success": false,
  "message": "Error message here"
}
```

---

## ?? Authorization

Ch? Admin m?i có th? access các endpoint này. ??m b?o user có role = "admin" trong database.

```sql
UPDATE "Accounts" 
SET "Role" = 'admin' 
WHERE "Email" = 'admin@example.com';
```
