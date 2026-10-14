## Test 

Login - `POST`
```
https://localhost:5001/api/auth/login
```

```
{
  "email": "task4_tester_20261009@example.com",
  "password": "PetPalTest1!"
}
```

Best-adopter - `GET`
```
https://localhost:5001/api/pets/1/best-adopter
```

<br>

**Other**

public endpoint
```
https://localhost:5001/api/pets?isAvailable=true
```
authenticated endpoint
```
https://localhost:5001/api/users/{userId}/pets
```
create request
```
https://localhost:5001/api/pets/{petId}/adoption-requests
```


<br>
