# Portable WinRT/.NET/Xamarin (PCL) experimental implementation of Javascript Object Signing and Encryption (JOSE) and JSON Web Token (JWT)

##Goal:
Attempt to re-implement portion of [jose-rt](https://github.com/dvsekhvalnov/jose-jwt) supported algorithms based on [PCLCrypto](https://github.com/AArnott/PCLCrypto/)
. This will allow to cross target single .nupkg for multiple platforms.

## Supported JWA algorithms

**Signing**
- HMAC signatures with HS256, HS384 and HS512.
- RSASSA-PKCS1-V1_5 signatures with RS256, RS384 and RS512.


##### Notes:
\* Actual supported algorithms depends on target platform. See [PCLCrypto compatibility list](https://github.com/AArnott/PCLCrypto/wiki/Algorithms-X-platforms-support).

## Which version?
* v0.0.3, depends on [PCLCrypto v1.0.86](https://github.com/AArnott/PCLCrypto) - if you targeting Win8.x Store, this is what you should use as of now.

* v0.0.4 and above depends on [PCLCrypto v2.0.147](https://github.com/AArnott/PCLCrypto) - if you app targeting Win8.x Store - it most likely will be rejected due to https://github.com/AArnott/PCLCrypto/issues/97. See https://github.com/dvsekhvalnov/jose-pcl/issues/4 for more discussions.

## Installation
### NuGet
https://www.nuget.org/packages/jose-pcl

`Install-Package jose-pcl`

### Manual
Grab source and compile yourself.

## Usage
### Creating Plaintext (unprotected) Tokens

```C#
string payload = @"{""hello"" : ""world""}";

string token = JosePCL.Jwt.Encode(payload, JwsAlgorithm.None, null);
```

### Creating signed Tokens
#### HS256, HS384 and HS512 family
HS256, HS384, HS512 signatures require `byte[]` array key of corresponding length

```C#
var secretKey = new byte[]{164,60,194,0,161,189,41,38,130,89,141,164,45,170,159,209,69,137,243,216,191,131,47,250,32,107,231,117,37,158,225,234};

string payload = @"{""hello"" : ""world""}";

string token = JosePCL.Jwt.Encode(payload, JwsAlgorithm.HS256, secretKey);
```

#### RS256, RS384, RS512 family
RS256, RS384, RS512 signatures require `PCLCrypto.ICryptographicKey` private key of corresponding length. `JosePCL` provides convenient helpers to load RSA keys from commonly
used PEM encoded formats. See [Obtaining keys](#obtaining-keys) section for details.

```C#
string payload = @"{""hello"" : ""world""}";

string privateKey=
@"-----BEGIN PRIVATE KEY-----
MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALx97GSHCGkevvUS
sXMscNd+08MjO8BbkrzzlDuokJzVvQQprSEFYCO1ojp1UheAImeQvMe1wAWrGNfb
Fw34jQCSkv8liWLh5aHqHPrU8DTgKsL+XjHGaMwsg8y68pEmZrpyV/N49yXKlh3C
1PLnFJrTmZq0PHLqOXINNvMWFv7jAgMBAAECgYEAsYc0RzY7AK7ZkX7KrLw1h3FH
R2n+09wrp1UOzuWjVmOkw6/xBMHIW7mtkrt+1u1y+fIDK2GN+oi8PEl4PEtVmI8L
jaExLu5fsp/Z+BbHfcs4L5So9pdGZn5Dhfh606LWRZ0qqSjdtXitpNMrjx736+Jt
J6/kHlCdmYDyThtljbECQQDoDDAznyi6Yl2T+taoi2VcCP7wFAIYf3Mu6nqiEBhc
p1lVOuWjyR+mBU8+o6hDs40oVAOdpCdqtDJ3ppWABKKZAkEAz/LIq8Uwq8ephNwn
WOSuhkjUz+O01v74GHyS6tc7WGckFR7JS1cughXlRRq7hD1z1dhTYq0W2g4Yrujf
GFTW2wJBAIwtQLkOfqYJYgpQz3fFrZdpf8g77gAqjcRbtXVNT8o49gg8qhjFGK9M
KdDnQHCVeMJR7lU+oukcrhgFs+4/3pECQBcvX5ZfPwT4Fvt8PFrZ7GeGeUvQfJo4
BVtdkFfktXYu0cQVEaZ3yvSwEkb5Kw0ceOzP2MQ4vkKDrdbamf0xgF8CQFiz2P8h
Vq/Q3fFKCWamZ1olx08zo4x4y2kYKO275GSZabhiVoulVhUtRgi9BcPfW9kakqps
wEe4//EeSbl38Bk=
-----END PRIVATE KEY-----"


string token = JosePCL.Jwt.Encode(payload, JwsAlgorithm.RS512, JosePCL.Keys.Rsa.PrivateKey.Load(privateKey));
```

### Verifying and Decoding Tokens
Decoding json web tokens is fully symmetric to creating signed or encrypted tokens:

**HS256, HS384, HS512** signatures, **A128KW, A192KW, A256KW** and **DIR** key management algorithms expecting `byte[]` array key

```C#
string token = "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2R0NNIn0..Fmz3PLVfv-ySl4IJ.LMZpXMDoBIll5yuEs81Bws2-iUUaBSpucJPL-GtDKXkPhFpJmES2T136Vd8xzvp-3JW-fvpRZtlhluqGHjywPctol71Zuz9uFQjuejIU4axA_XiAy-BadbRUm1-25FRT30WtrrxKltSkulmIS5N-Nsi_zmCz5xicB1ZnzneRXGaXY4B444_IHxGBIS_wdurPAN0OEGw4xIi2DAD1Ikc99a90L7rUZfbHNg_iTBr-OshZqDbR6C5KhmMgk5KqDJEN8Ik-Yw.Jbk8ZmO901fqECYVPKOAzg";

byte[] secretKey = new byte[]{164,60,194,0,161,189,41,38,130,89,141,164,45,170,159,209,69,137,243,216,191,131,47,250,32,107,231,117,37,158,225,234};

string json = JosePCL.Jwt.Decode(token, secretKey);
```

**RS256, RS384, RS512** signatures expecting `PCLCrypto.ICryptographicKey` public key of corresponding length. `JosePCL` provides convenient helpers to load RSA keys from commonly
used PEM encoded formats. See [Obtaining keys](#obtaining-keys) section for details.

```C#
string token = "eyJhbGciOiJSUzM4NCIsImN0eSI6InRleHRcL3BsYWluIn0.eyJoZWxsbyI6ICJ3b3JsZCJ9.cOPca7YEOxnXVdIi7cJqfgRMmDFPCrZG1M7WCJ23U57rAWvCTaQgEFdLjs7aeRAPY5Su_MVWV7YixcawKKYOGVG9eMmjdGiKHVoRcfjwVywGIb-nuD1IBzGesrQe7mFQrcWKtYD9FurjCY1WuI2FzGPp5YhW5Zf4TwmBvOKz6j2D1vOFfGsogzAyH4lqaMpkHpUAXddQxzu8rmFhZ54Rg4T-jMGVlsdrlAAlGA-fdRZ-V3F2PJjHQYUcyS6n1ULcy6ljEOgT5fY-_8DDLLpI8jAIdIhcHUAynuwvvnDr9bJ4xIy4olFRqcUQIHbcb5-WDeWul_cSGzTJdxDZsnDuvg";

var publicKey=
@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqFZv0pea/jn5Mo4qEUmS
tuhlulso8n1inXbEotd/zTrQp9K0RK0hf7t0K4BjKVhaiqIam4tVVQvkmYeBeYr1
MmnO/0N97dMBz/7fmvyv0hgHaBdQ5mR5u3LTlHo8tjRE7+GzZmGs6jMcyj7HbXob
DPQJZpqNy6JjliDVXxW8nWJDetxGBlqmTj1E1fr2RCsZLreDOPSDIedG1upz9Rra
ShsIDzeefOcKibcAaKeeVI3rkAU8/mOauLSXv37hlk0h6sStJb3qZQXyOUkVkjXI
khvNu/ve0v7LiLT4G/OxYGzpOQcCnimKdojzNP6GtVDaMPh+QkSJE32UCos9R3wI
2QIDAQAB
-----END PUBLIC KEY-----";

string json = Jwt.Decode(token, JosePCL.Keys.Rsa.PublicKey.Load(publicKey));
```

### Obtaining keys
`JosePCL` provides set of helpers to import PEM encoded keys or other formats.

#### RSA keys
`JosePCL.Keys.Rsa.PublicKey.Load(string)` imports `PCLCrypto.ICryptographicKey` from PEM encoded public key formats

##### PKCS#1 RSA Public Key

	-----BEGIN RSA PUBLIC KEY-----
	MIIBCgKCAQEAqFZv0pea/jn5Mo4qEUmS
	tuhlulso8n1inXbEotd/zTrQp9K0RK0hf7t0K4BjKVhaiqIam4tVVQvkmYeBeYr1
	MmnO/0N97dMBz/7fmvyv0hgHaBdQ5mR5u3LTlHo8tjRE7+GzZmGs6jMcyj7HbXob
	DPQJZpqNy6JjliDVXxW8nWJDetxGBlqmTj1E1fr2RCsZLreDOPSDIedG1upz9Rra
	ShsIDzeefOcKibcAaKeeVI3rkAU8/mOauLSXv37hlk0h6sStJb3qZQXyOUkVkjXI
	khvNu/ve0v7LiLT4G/OxYGzpOQcCnimKdojzNP6GtVDaMPh+QkSJE32UCos9R3wI
	2QIDAQAB
	-----END RSA PUBLIC KEY-----

##### X509 Public Subject key info
Can be obtained from certificate via `openssl x509 -inform PEM -in certificate.cer -outform PEM -pubkey -noout > public.key`

	-----BEGIN PUBLIC KEY-----
	MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqFZv0pea/jn5Mo4qEUmS
	tuhlulso8n1inXbEotd/zTrQp9K0RK0hf7t0K4BjKVhaiqIam4tVVQvkmYeBeYr1
	MmnO/0N97dMBz/7fmvyv0hgHaBdQ5mR5u3LTlHo8tjRE7+GzZmGs6jMcyj7HbXob
	DPQJZpqNy6JjliDVXxW8nWJDetxGBlqmTj1E1fr2RCsZLreDOPSDIedG1upz9Rra
	ShsIDzeefOcKibcAaKeeVI3rkAU8/mOauLSXv37hlk0h6sStJb3qZQXyOUkVkjXI
	khvNu/ve0v7LiLT4G/OxYGzpOQcCnimKdojzNP6GtVDaMPh+QkSJE32UCos9R3wI
	2QIDAQAB
	-----END PUBLIC KEY-----

`JosePCL.Keys.Rsa.PrivateKey.Load(string)` imports `PCLCrypto.ICryptographicKey` from PEM encoded private key formats

##### PKCS#1 RSA Private Key without password protection
Can be obtained from .p12 via `openssl pkcs12 -in keys.p12 -nocerts -out privateKey.pem` and then `openssl.exe rsa -in privateKey.pem -out privateKey.pem` to remove
password protection.

	-----BEGIN RSA PRIVATE KEY-----
	MIICXQIBAAKBgQC8fexkhwhpHr71ErFzLHDXftPDIzvAW5K885Q7qJCc1b0EKa0h
	BWAjtaI6dVIXgCJnkLzHtcAFqxjX2xcN+I0AkpL/JYli4eWh6hz61PA04CrC/l4x
	xmjMLIPMuvKRJma6clfzePclypYdwtTy5xSa05matDxy6jlyDTbzFhb+4wIDAQAB
	AoGBALGHNEc2OwCu2ZF+yqy8NYdxR0dp/tPcK6dVDs7lo1ZjpMOv8QTByFu5rZK7
	ftbtcvnyAythjfqIvDxJeDxLVZiPC42hMS7uX7Kf2fgWx33LOC+UqPaXRmZ+Q4X4
	etOi1kWdKqko3bV4raTTK48e9+vibSev5B5QnZmA8k4bZY2xAkEA6AwwM58oumJd
	k/rWqItlXAj+8BQCGH9zLup6ohAYXKdZVTrlo8kfpgVPPqOoQ7ONKFQDnaQnarQy
	d6aVgASimQJBAM/yyKvFMKvHqYTcJ1jkroZI1M/jtNb++Bh8kurXO1hnJBUeyUtX
	LoIV5UUau4Q9c9XYU2KtFtoOGK7o3xhU1tsCQQCMLUC5Dn6mCWIKUM93xa2XaX/I
	O+4AKo3EW7V1TU/KOPYIPKoYxRivTCnQ50BwlXjCUe5VPqLpHK4YBbPuP96RAkAX
	L1+WXz8E+Bb7fDxa2exnhnlL0HyaOAVbXZBX5LV2LtHEFRGmd8r0sBJG+SsNHHjs
	z9jEOL5Cg63W2pn9MYBfAkBYs9j/IVav0N3xSglmpmdaJcdPM6OMeMtpGCjtu+Rk
	mWm4YlaLpVYVLUYIvQXD31vZGpKqbMBHuP/xHkm5d/AZ
	-----END RSA PRIVATE KEY-----

##### PKCS#8 Raw RSA Private Key
Can be converted from PKCS#1 private key via `openssl pkcs8 -topk8 -inform PEM -outform PEM -in privateKey.pem -out privateKey.key -nocrypt`

	-----BEGIN PRIVATE KEY-----
	MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALx97GSHCGkevvUS
	sXMscNd+08MjO8BbkrzzlDuokJzVvQQprSEFYCO1ojp1UheAImeQvMe1wAWrGNfb
	Fw34jQCSkv8liWLh5aHqHPrU8DTgKsL+XjHGaMwsg8y68pEmZrpyV/N49yXKlh3C
	1PLnFJrTmZq0PHLqOXINNvMWFv7jAgMBAAECgYEAsYc0RzY7AK7ZkX7KrLw1h3FH
	R2n+09wrp1UOzuWjVmOkw6/xBMHIW7mtkrt+1u1y+fIDK2GN+oi8PEl4PEtVmI8L
	jaExLu5fsp/Z+BbHfcs4L5So9pdGZn5Dhfh606LWRZ0qqSjdtXitpNMrjx736+Jt
	J6/kHlCdmYDyThtljbECQQDoDDAznyi6Yl2T+taoi2VcCP7wFAIYf3Mu6nqiEBhc
	p1lVOuWjyR+mBU8+o6hDs40oVAOdpCdqtDJ3ppWABKKZAkEAz/LIq8Uwq8ephNwn
	WOSuhkjUz+O01v74GHyS6tc7WGckFR7JS1cughXlRRq7hD1z1dhTYq0W2g4Yrujf
	GFTW2wJBAIwtQLkOfqYJYgpQz3fFrZdpf8g77gAqjcRbtXVNT8o49gg8qhjFGK9M
	KdDnQHCVeMJR7lU+oukcrhgFs+4/3pECQBcvX5ZfPwT4Fvt8PFrZ7GeGeUvQfJo4
	BVtdkFfktXYu0cQVEaZ3yvSwEkb5Kw0ceOzP2MQ4vkKDrdbamf0xgF8CQFiz2P8h
	Vq/Q3fFKCWamZ1olx08zo4x4y2kYKO275GSZabhiVoulVhUtRgi9BcPfW9kakqps
	wEe4//EeSbl38Bk=
	-----END PRIVATE KEY-----
