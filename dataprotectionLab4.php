<?
class CRSA
{
 
    // Два простых числа p и q
    private $p=0x0;
    private $q=0x0;
 
 
    // соответственно значение функции n(p,q), ph(p,q) этих простых чисел
    private $ph=0x0;
    private $n=0x0;
 
 
    // Ключи шифрования и дешифровки
    private $e=0x0;
    private $d=0x0;
 
    // Расшифрованая строка и вектор с зашифрованными символами
    private $decriptedData="";
    private $encriptedData=array();
 
 
 
    function __construct($p=0x0, $q=0x0)
    {
        $this->initPQParams($p, $q);
    }
 
    // Инициация функций n и ph
    function initPQParams($p=0x0, $q=0x0)
    {
        $this->p=$p;
        $this->q=$q;
 
        if ($this->p && $this->q && ($this->q!=$this->p))
        {
            $this->n=$this->p*$this->q;
            $this->ph=($this->p-1)*($this->q-1);
        }
    }
 
    // Инициирование подходящего ключа шифрования
    function initEncryptingKey($e=0x0)
    {
 
        if (!$this->ph || !$e)
            return false;
 
        $this->e=$e;
 
        while (!$this->isMutuallyDistinct($this->e, $this->ph))
            $this->e+=2;
 
        return true;
    }
 
 
    // Проверка на чисел на взаимную простоту
    private function isMutuallyDistinct($a, $b)
    {
        while(($a!=0x0) && ($b!=0x0))
            if($a>=$b) $a=$a%$b;
                else $b=$b%$a;
 
        return (($a+$b)==1)?true:false;
    }
 
    // Реализация расширеного алгоритма Эвклида для поиска
    // ключа дешифрования
    function generateDecripringKey()
    {
        $u1=1;
        $u2=0x0;
        $u3=$this->ph;
        $r1=0x0;
        $r2=1;
        $r3=$this->e;
 
        while ($r3)
        {
            if(!$r3)
             break;
 
            $q=(int)(((int)$u3)/((int)$r3));
            $t1=($u1-$r1*$q);
            $t2=($u2-$r2*$q);
            $t3=($u3-$r3*$q);
            $u1=$r1;
            $u2=$r2;
            $u3=$r3;
            $r1=$t1;
            $r2=$t2;
            $r3=$t3;
        }
        $this->d=$u2;
 
    }
 
    
    function getEncripringKey()
    {
        return $this->e;
    }
 
    
    function getDecriptingKey($forced=false)
    {
        if ((!$this->d) || ($forced))
        {
            $this->d=0x0;
            $this->generateDecripringKey();
        }
        return $this->d;
    }
 
 
    // Ф-я шифрования, может работать опираясь как на ранее 
    // ининциированные свойства, так и напрямую от параметров -   
    // ключа шифрации и занчения n(p,q);
    function EncriptX($data, $e=0x0, $n=0x0)
    {
        if ($e>0x0 && $n>0x0)
        {
            $this->n=$n;
            $this->e=$e;
        }
 
        for ($j=0x0; $j<strlen($data); $j++)
        {
            $b=ord($data[$j]);
            $result = 1;
            for($i=0x0; $i<$this->e; $i++)
            {
                $result = ($result*$b) % $this->n;
            }
            echo $this->encriptedData[$j]=$result;
        }

        //return $this->encriptedData;
    }
 
    // Аналогично ф-ии шифрования
    function DecriptX($d=0x0, $n=0x0)
    {
        if ($d>0x0 && $n>0x0)
        {
            $this->d=$d;
            $this->n=$n;
        }
 
        $result = 1;
        for ($j=0x0; $j<count($this->encriptedData); $j++)
        {
            $b=($this->encriptedData[$j]);
            $result = 1;
            for($i=0x0; $i<$this->d; $i++)
            {
                $result = ($result*$b) % $this->n;
            }
            $this->decriptedData .= chr($result);
        }
 
        return $this->decriptedData;
    }
}

$rsa=new CRSA();
echo "Зашифровка: <br>";
$rsa->EncriptX("Это алгоритм RSA", 79, 3337);
echo "\n";
echo "<br>Расшифровка: <br>";
echo $rsa->DecriptX(1019, 3337);
echo "\n";