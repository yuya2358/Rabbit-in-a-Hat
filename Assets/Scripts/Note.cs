/*
 * 20231014.2217: 
 * 문제: Rabbit이 점프를 할 수 없음.
 * 원인: 참고했던 슈퍼마리오 예제에 쓰인 마리오의 Capsule Collider 2D는 Size가 X1:Y1 였던 반에 내가 쓴 것은 X1:Y2로 되어 있어 두 Collision이 충돌한 것으로 보임.
 * 해결: Size를 X1:Y1으로 바꾸니 점프가 가능해졌으나 Rabbit의 하반부가 바닥에 꺼지는 상황이 생김. Raycasting의 distance를 수정하여 해결. 
 * 
 * 20231014.2315: 
 * 문제: 카메라 떨림. 
 * 원인: 예제에 쓰인 LateUpdate
 * 해결: LateUpdate를 FixedUpdate로 바꿈. 
 */